const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const root=require('node:path').resolve(__dirname, '..');
const ts=require(root+'/node_modules/typescript');
const zod=require(root+'/node_modules/zod');
function load(file,imports={},globals={},env={}) {
 const source=fs.readFileSync(root+'/src/features/rooms/'+file,'utf8').replace(/import.meta.env.(VITE_\w+)/g,(_,key)=>JSON.stringify(env[key]) ?? 'undefined');
 const exports={};
 vm.runInNewContext(ts.transpileModule(source,{compilerOptions:{module:ts.ModuleKind.CommonJS,target:ts.ScriptTarget.ES2022}}).outputText,
 {exports,require:id=>{if(id==='zod')return zod; assert.ok(imports[id],id);return imports[id];},Date,Math,Map,Set,URLSearchParams,...globals});
 return exports;
}
const contract=load('contract.ts');
const calendar=load('calendar.ts');
const calls=[];
let response;
const storage=new Map();
const localStorage={getItem:key=>storage.get(key)??null,setItem:(key,value)=>storage.set(key,String(value)),removeItem:key=>storage.delete(key)};
const api=load('api.ts',{'./contract':contract,'../../lib/api':{apiFetch:async(...args)=>{calls.push(args);return response;}}},{localStorage});
async function run(){
 const now=new Date(2026,9,15,12);
 const dates=calendar.getWeekDates(0,now);
 assert.equal(dates[0].getDay(),1); assert.equal(dates[0].getDate(),12); assert.equal(dates[4].getDate(),16);
 assert.equal(calendar.getWeekDates(1,now)[0].getDate(),19);
 const range=calendar.getBookingRange(dates);
 assert.equal(new Date(range.toUtc).getDay(),6); assert.equal(new Date(range.toUtc).getHours(),0);
 console.log('PASS real Monday-Friday calendar and exclusive local-to-UTC query range');
 const start=calendar.toBookingStart(dates[3],14*60);
 assert.equal(start.getHours(),14);assert.equal(start.getDate(),15);
 const record={id:4,roomId:1,organizerEmployeeId:9,title:'Test',startDateTimeUtc:start.toISOString(),durationMinutes:60};
 contract.bookingSchema.parse(record);
 let mapped=calendar.toCalendarBookings([record],dates,new Set([1]));
 assert.equal(mapped.length,1);assert.equal(mapped[0].day,3);assert.equal(mapped[0].startMin,840);
 assert.equal(calendar.toCalendarBookings([record],calendar.getWeekDates(1,now),new Set([1])).length,0);
 assert.equal(calendar.toCalendarBookings([record],dates,new Set([2])).length,0);
 console.log('PASS UTC/local roundtrip, filtering by displayed week and office rooms');
 const overnight={...record,startDateTimeUtc:calendar.toBookingStart(dates[2],18*60).toISOString(),durationMinutes:16*60};
 mapped=calendar.toCalendarBookings([overnight],dates,new Set([1]));
 assert.equal(mapped.length,2);assert.equal(mapped[0].durationMin,60);assert.equal(mapped[1].durationMin,120);
 console.log('PASS cross-day booking clips to visible hours without overflowing calendar');
 response=[{id:1,name:'Test Room',floor:2,seats:8,amenities:null,locationId:1}];
 const rooms=await api.getRooms();assert.equal(calendar.toRoom(rooms[0]).meta,'Seats 8 · floor 2');
 response=[record];await api.getRoomBookings(range.fromUtc,range.toUtc);
 const url=new URL(calls.at(-1)[0],'http://test');assert.equal(url.searchParams.get('fromUtc'),range.fromUtc);assert.equal(url.searchParams.get('toUtc'),range.toUtc);
 response=undefined;
 const input={roomId:1,title:'Test',startDateTimeUtc:start.toISOString(),durationMinutes:60};
 await api.createRoomBooking(input);assert.deepEqual(JSON.parse(calls.at(-1)[1].body),input);assert.equal(calls.at(-1)[1].method,'POST');
 console.log('PASS proposed list routes, UTC query and create payload; no-body creation response');
 response={items:[]};await assert.rejects(()=>api.getRooms());
 assert.throws(()=>contract.bookingSchema.parse({...record,startDateTimeUtc:'2026-10-15T14:00:00'}));
 console.log('PASS incompatible response and timezone-free timestamp are rejected');
 const customApi=load('api.ts',{'./contract':contract,'../../lib/api':{apiFetch:async(...args)=>{calls.push(args);return args[1]?.method==='POST'?undefined:[];}}},{},{VITE_ROOMS_API_PATH:'/custom/rooms/'});
 await customApi.getRooms();assert.equal(calls.at(-1)[0],'/custom/rooms');
 const roomInput={name:'New room',locationId:1,floor:2,seats:6};
 await customApi.createRoom(roomInput);assert.equal(calls.at(-1)[0],'/custom/rooms');assert.equal(calls.at(-1)[1].method,'POST');assert.deepEqual(JSON.parse(calls.at(-1)[1].body),roomInput);
 console.log('PASS room creation and listing share the configured API route');
 const {QueryClient,MutationObserver}=require(root+'/node_modules/@tanstack/react-query');
 const client=new QueryClient({defaultOptions:{queries:{retry:false},mutations:{retry:false}}});
 const roomKey=['room-bookings','from','to'];const homeKey=['home-meetings','/RoomBookings','from','to'];const unrelatedKey=['employees'];
 client.setQueryData(roomKey,[]);client.setQueryData(homeKey,[]);client.setQueryData(unrelatedKey,[]);
 const hooks=load('useRooms.ts',{'@tanstack/react-query':{useQueryClient:()=>client,useMutation:options=>options},'./api':{createRoomBooking:async()=>undefined}});
 try {
  await new MutationObserver(client,hooks.useCreateRoomBooking()).mutate(input);
  assert.equal(client.getQueryState(roomKey).isInvalidated,true);
  assert.equal(client.getQueryState(homeKey).isInvalidated,true);
  assert.equal(client.getQueryState(unrelatedKey).isInvalidated,false);
 } finally {client.clear();}
 console.log('PASS successful booking invalidates both calendar and Home meeting caches');
}
run().catch(error=>{console.error(error);process.exitCode=1;});
