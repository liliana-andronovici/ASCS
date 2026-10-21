const fs=require('node:fs');const path=require('node:path');const vm=require('node:vm');const assert=require('node:assert/strict');
const root=path.resolve(__dirname,'..');
const ts=require(path.join(root,'node_modules/typescript'));const zod=require(path.join(root,'node_modules/zod'));
process.env.TZ='Europe/Chisinau';
function load(file,imports={},globals={}){
 const source=fs.readFileSync(path.join(root,'src/features/home',file),'utf8').replace(/import.meta.env.VITE_\w+/g,'undefined');
 const exports={};vm.runInNewContext(ts.transpileModule(source,{compilerOptions:{module:ts.ModuleKind.CommonJS,target:ts.ScriptTarget.ES2022}}).outputText,
 {exports,require:id=>{if(id==='zod')return zod;assert.ok(imports[id],id);return imports[id];},Date,Map,Set,Math,Number,URLSearchParams,...globals});return exports;
}
const calendar=load('calendar.ts');const contracts=load('contracts.ts');
const calculations=load('calculations.ts',{'./calendar':calendar});let request;
const storage=new Map();
const localStorage={getItem:key=>storage.get(key)??null,setItem:(key,value)=>storage.set(key,String(value)),removeItem:key=>storage.delete(key)};
const api=load('api.ts',{'./contracts':contracts,'../../lib/api':{apiFetch:(...args)=>request(...args)}},{localStorage});
const employee=(id,startedOnUtc='2020-01-01T00:00:00Z',status='Active')=>({id,firstName:'Test',lastName:String(id),email:`test-${id}@example.com`,title:'Test role',startedOnUtc,departmentId:null,locationId:null,managerId:null,status});
async function run(){
 const now=new Date(2026,9,15,12);
 const records=[employee(1),employee(2,'2026-10-01T00:00:00Z'),employee(3,'2026-10-20T00:00:00Z'),employee(4,'2025-10-16T00:00:00Z'),employee(5,'2020-01-01T00:00:00Z','Inactive')];
 const stats=calculations.employeeStats(records,now);assert.equal(stats.headcount,3);assert.equal(stats.joinedThisMonth,1);assert.equal(stats.startingThisMonth,2);
 const upcoming=calculations.buildUpcoming(records,now);assert.ok(upcoming.some(item=>item.id==='start-3'));assert.ok(upcoming.some(item=>item.id==='anniversary-4'));assert.ok(!upcoming.some(item=>item.id.endsWith('-5')));
 console.log('PASS active headcount excludes future starters; monthly totals and upcoming dates');
 const leave={id:1,employeeId:1,startDateUtc:'2026-10-15',endDateUtc:'2026-10-18',type:'Vacation',status:'Approved'};
 assert.equal(calculations.absentEmployees([leave,{...leave,id:2}],records,now).length,1);
 assert.equal(calculations.absentEmployees([leave],records,new Date(2026,9,18,12)).length,1);
 assert.equal(calculations.absentEmployees([{...leave,status:'Pending'}],records,now).length,0);
 console.log('PASS absences deduplicate employees, count weekends and exclude pending requests');
 const meeting={id:1,title:'Overnight',startUtc:new Date(2026,9,15,23).toISOString(),endUtc:new Date(2026,9,16,1).toISOString(),location:null};
 const days=calculations.buildMeetingsWeek([meeting],now);assert.equal(days[3].meetings.length,1);assert.equal(days[4].meetings.length,1);
 const range=calendar.weekRange(now);assert.equal(new Date(range.to).getDay(),6);assert.equal(new Date(range.to).getHours(),0);
 assert.equal(new Date(calendar.weekRange(now,true).to).getDay(),1);
 const afterDst=new Date(2026,9,26,12);const monday=calendar.mondayOf(afterDst);assert.equal(monday.getDay(),1);assert.equal(monday.getHours(),0);
 const beforeDst=new Date(2026,9,23);assert.equal(calendar.addDays(beforeDst,4).getHours(),0);
 console.log('PASS overnight meetings, exclusive Friday bounds and local midnight across DST');
 const employees=Array.from({length:503},(_,i)=>employee(i+1));let pages=0;
 request=async route=>{pages++;const url=new URL(route,'http://test');const page=Number(url.searchParams.get('page'));assert.equal(url.searchParams.get('sortBy'),'Id');return {items:employees.slice((page-1)*70,page*70),page,pageSize:70,totalCount:503};};
 assert.equal((await api.getHomeEmployees()).length,503);assert.equal(pages,8);
 console.log('PASS all employee pages load beyond 500 records, including server page-size caps');
 request=async()=>({items:[],page:1,pageSize:100,totalCount:0});assert.equal((await api.getHomeEmployees()).length,0);
 request=async()=>({items:[],page:1,pageSize:100,totalCount:5});await assert.rejects(()=>api.getHomeEmployees());
 request=async()=>({items:[employee(1),employee(1)],page:1,pageSize:100,totalCount:2});await assert.rejects(()=>api.getHomeEmployees());
 request=async()=>({items:[{...employee(1),startedOnUtc:'invalid'}],page:1,pageSize:100,totalCount:1});await assert.rejects(()=>api.getHomeEmployees());
 console.log('PASS empty directory is valid; incomplete, duplicate and malformed results fail');
 assert.equal(api.HOME_LEAVE_PATH,'/TimeOff/out-today');assert.equal(api.HOME_MEETINGS_PATH,'/RoomBookings');
 request=async route=>{const url=new URL(route,'http://test');assert.equal(url.pathname,'/TimeOff/out-today');assert.equal(url.searchParams.get('date'),'2026-10-15');return {count:2};};
 assert.equal((await api.getHomeOutToday('2026-10-15')).count,2);
 request=async route=>{const url=new URL(route,'http://test');assert.equal(url.pathname,'/RoomBookings');assert.equal(url.searchParams.get('fromUtc'),range.from);assert.equal(url.searchParams.get('toUtc'),range.to);return [{id:7,title:'Planning',roomName:'Loft',organizerEmployeeId:32,startDateTimeUtc:'2026-10-15T10:00:00.000Z',durationMinutes:30},{id:8,title:'Someone else',roomName:'Loft',organizerEmployeeId:2,startDateTimeUtc:'2026-10-15T11:00:00.000Z',durationMinutes:30}];};
 const meetings=await api.getHomeMeetings(range.from,range.to,32);assert.equal(meetings.length,1);assert.equal(meetings[0].endUtc,'2026-10-15T10:30:00.000Z');assert.equal(meetings[0].location,'Meeting room · Loft');
 console.log('PASS dashboard uses the time off count and room booking feeds');
}
run().catch(error=>{console.error(error);process.exitCode=1;});
