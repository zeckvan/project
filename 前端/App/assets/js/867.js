"use strict";(self["webpackChunkportfolio"]=self["webpackChunkportfolio"]||[]).push([[867],{2872:function(t,e,a){a.r(e),a.d(e,{default:function(){return O}});var s=function(){var t=this,e=t._self._c;return e("div",[e("PubQuery",{on:{"get-condition":t.getcondition}}),e("el-tabs",{on:{"tab-click":t.handleClick},model:{value:t.activeName,callback:function(e){t.activeName=e},expression:"activeName"}},[e("el-tab-pane",{attrs:{label:"課程諮詢資料建立",name:"first"}},[e("TeaConsultGrid",{attrs:{userStatic:t.userStatic,api_interface:t.api_interface,tableData:t.tableData,total:t.pagetotal1},on:{"get-studata":t.getstudata}})],1),e("el-tab-pane",{attrs:{label:"課程諮詢學生建立",name:"second"}},[e("TeaConsultStu",{attrs:{userStatic:t.userStatic,api_interface:t.api_interface,parentData:t.stuData,total:t.pagetotal2,complex_key:t.complex_key,tea_consult:t.tea_consult}})],1)],1)],1)},r=[],o=a(5705),i=function(){var t=this,e=t._self._c;return e("div",[e("div",{attrs:{align:"left"}},[e("el-button",{attrs:{type:"primary"},on:{click:t.add}},[t._v("新增課程諮詢")])],1),e("el-table",{staticStyle:{width:"100%"},attrs:{data:t.tableData,height:"63vh",stripe:"","row-style":t.rowState},on:{"row-click":t.rowClick}},[e("el-table-column",{attrs:{type:"index",width:"50"}}),t._l(t.render_header,(function(t){return e("el-table-column",{key:t.prop,attrs:{prop:t.prop,label:t.label,width:t.width}})})),e("el-table-column",{attrs:{align:"center",fixed:"right",label:"檢視資料",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("i",{staticClass:"el-icon-search",staticStyle:{cursor:"pointer"},on:{click:function(e){return t.edit_data(a)}}})]}}])}),e("el-table-column",{attrs:{align:"center",fixed:"right",label:"上傳檔案",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("i",{staticClass:"el-icon-upload",staticStyle:{cursor:"pointer"},on:{click:function(e){return t.file_upload(a)}}})]}}])}),e("el-table-column",{attrs:{align:"center",fixed:"right",label:"查看檔案",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("el-badge",{staticClass:"item",staticStyle:{cursor:"pointer"},attrs:{value:a.row.x_cnt,type:"info"},nativeOn:{click:function(e){return t.file_data(a)}}})]}}])}),e("el-table-column",{attrs:{align:"center",fixed:"right",label:"刪除資料",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("i",{staticClass:"el-icon-delete",staticStyle:{cursor:"pointer"},on:{click:function(e){return t.del_data(a)}}})]}}])})],2),e("el-pagination",{attrs:{"page-size":t.pageSize,total:t.total,layout:"total,prev, pager, next,sizes"},on:{"current-change":t.current_change,"size-change":t.size_change}}),t.isShow?e(t.content,{tag:"component",attrs:{dialog_show:t.dialog_show,edit_model:t.edit_model,parameter:t.parameter,data_structure:t.data_structure,userStatic:t.userStatic,api_interface:t.api_interface},on:{"update:dialog_show":function(e){t.dialog_show=e},"get-show":t.getshow}}):t._e(),t.isShow2?e("v-stufilelist",{attrs:{dialog_show:t.dialog_show2,filelist:t.filelist,userStatic:t.userStatic,parameter:t.parameter,api_interface:t.api_interface},on:{"update:dialog_show":function(e){t.dialog_show2=e},"get-show":t.getshow2}}):t._e(),t.isShow3?e("v-fileupload",{attrs:{dialog_show:t.dialog_show3,filelist:t.filelist,userStatic:t.userStatic,api_interface:t.api_interface,parameter:t.parameter,data_structure:t.data_structure,rowdata:t.rowdata},on:{"update:dialog_show":function(e){t.dialog_show3=e},"get-show":t.getshow3}}):t._e()],1)},n=[],l=a(4442),c=a(2301),u=a(437),d=a(4876),p="",h={props:{userStatic:{type:Object},api_interface:{type:Object},tableData:{type:Array},total:{type:Number}},data(){return{content:this.userStatic.form_component,data_structure:{},isShow:!1,isShow2:!1,isShow3:!1,dialog_show:!0,dialog_show2:!0,dialog_show3:!0,currentPage:1,pageSize:10,year:"",sms:"",edit_model:"",parameter:{},filelist:[],rowdata:{}}},methods:{rowClick(t,e,a){this.$emit("get-studata",t)},rowState(t,e){return{backgroundColor:"#f4f4f5"}},add:function(){this.edit_model="add",this.isShow=!0},edit_data:function(t){let e=this;this.data_structure.header.forEach((function(a,s,r){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])})),this.edit_model="edit",this.isShow=!0},file_data:function(t){let e=this;p=e.api_interface.file_data;const a=e.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});e.data_structure.header.forEach((function(a,s,r){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])}));let s=`${t.row.a}_${t.row.b}_${t.row.c}_${t.row.d}_${t.row.e}`;e.parameter["complex_key"]=s,e.parameter["class_name"]=e.userStatic.interface,e.parameter.sRowNun=1,e.parameter.eRowNun=999,e.$http({url:p,method:"get",params:e.parameter,headers:{SkyGet:e.$token}}).then((t=>{"Y"==t.data.status?e.filelist=t.data.dataset:(e.filelist=[],e.$message.error("查無資料，請確認"))})).catch((t=>{e.tableData=[],e.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>a.close())),this.isShow2=!0},file_upload:function(t){let e=this;p=e.api_interface.file_upload,e.rowdata=t,e.data_structure.header.forEach((function(a,s,r){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])})),this.isShow3=!0},del_data:function(t){let e=this;if("Y"===this.data_structure.delete_rule.rule_flag&&!this.data_structure.delete_rule.rule_check(t,e))return!1;e.$confirm("確定刪除?","Warning",{confirmButtonText:"確定",cancelButtonText:"取消",type:"warning"}).then((()=>{const a=e.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});let s=new FormData;e.data_structure.header.forEach((function(a,r,o){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop],s.append(a.col,t.row[a.prop]))})),s.append("class_name",e.userStatic.interface),p=e.api_interface.del_data,e.$http({headers:{"Content-Type":"application/json;charset=utf-8",SkyGet:e.$token},url:p,method:"delete",data:s}).then((a=>{"Y"==a.data.status?(e.$message.success("刪除成功!!"),e.tableData.splice(t.$index,1)):e.$message.error("刪除失敗!!")})).catch((t=>{e.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>a.close()))})).catch((()=>{}))},getshow:function(t){this.isShow=t.show},getshow2:function(t){this.isShow2=t.show},getshow3:function(t){this.isShow3=t.show},getcondition:function(t){var e=this;e.year=t.year,e.sms=t.sms,p=e.api_interface.get_data,this.get_data(p,t.year,t.sms,e.currentPage,e.pageSize)},current_change(t){var e=this,a="",s="";a=(t-1)*e.pageSize+1,s=t*e.pageSize,e.currentPage=t,p=e.api_interface.get_data,this.get_data(p,e.year,e.sms,a,s)},size_change(t){var e=this,a="",s="";a=(e.currentPage-1)*t+1,s=e.currentPage*t,e.pageSize=t,p=e.api_interface.get_data,this.get_data(p,e.year,e.sms,a,s)},get_data:function(t,e,a,s,r){let o=this;const i=o.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});o.parameter.emp_id="",o.parameter.year_id=e,o.parameter.sms_id=a,o.parameter.sRowNun=s,o.parameter.eRowNun=r,o.parameter.sch_no="",o.$http({url:t,method:"get",params:o.parameter,headers:{SkyGet:o.$token}}).then((t=>{"Y"==t.data.status?(o.tableData=t.data.dataset,o.total=t.data.dataset[0].x_cnt):(o.tableData=[],o.$message.error("查無資料，請確認"))})).catch((t=>{console.log(t),o.tableData=[],o.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>i.close()))}},components:{"v-teaconsultform":c.Z,"v-pubquery":o.Z,"v-stufilelist":l.Z,"v-fileupload":u.Z},beforeDestroy(){},mounted(){},beforeMount(){switch(this.userStatic.data_structure){case"teaconsult":this.data_structure=d.H_;break;default:}},computed:{render_header(){let t=this.data_structure.header.filter((t=>"N"===t.hidden)).sort((function(t,e){return t.sort>e.sort?1:-1}));return t}}},_=h,m=a(1001),g=(0,m.Z)(_,i,n,!1,null,null,null),f=g.exports,b=function(){var t=this,e=t._self._c;return e("div",[e("el-tag",{staticStyle:{"font-size":"30px"},attrs:{type:"info",size:"medium"}},[t._v(t._s(t.tag))]),e("div",{attrs:{align:"left"}},[e("el-button",{attrs:{type:"primary"},on:{click:t.add}},[t._v("新增諮詢學生")]),e("el-button",{attrs:{type:"danger"},on:{click:t.del_batch}},[t._v("批次刪除諮詢學生")])],1),e("el-table",{staticStyle:{width:"100%"},attrs:{data:t.tableData,stripe:"","row-style":t.rowState,height:"60vh"},on:{"header-click":t.headerclick,"cell-mouse-enter":t.cellmouseenter}},[e("el-table-column",{attrs:{type:"index",width:"50"}}),e("el-table-column",{staticStyle:{cursor:"pointer"},attrs:{prop:"x_status",width:"80","header-align":"center",align:"center",label:"全選"},scopedSlots:t._u([{key:"default",fn:function(a){return[e("el-checkbox",{model:{value:a.row.x_status,callback:function(e){t.$set(a.row,"x_status",e)},expression:"scope.row.x_status"}})]}}])}),t._l(t.render_header,(function(t){return e("el-table-column",{key:t.prop,attrs:{prop:t.prop,label:t.label,width:t.width}})})),e("el-table-column",{attrs:{align:"center",fixed:"right",label:"刪除資料",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("i",{staticClass:"el-icon-delete",staticStyle:{cursor:"pointer"},on:{click:function(e){return t.del_single(a)}}})]}}])})],2),e("el-pagination",{attrs:{"page-size":t.pageSize,total:t.total,layout:"total,prev, pager, next,sizes"},on:{"current-change":t.current_change,"size-change":t.size_change}}),t.isShow?e("PubStuList",{attrs:{dialog_show:t.dialog_show,userStatic:t.userStatic,api_interface:t.api_interface,data_structure:t.data_structure,parameter:t.parameter,tea_consult:t.tea_consult},on:{"update:dialog_show":function(e){t.dialog_show=e},"get-show":t.getshow,"get-data":t.getstudata}}):t._e()],1)},w=[],y=(a(7658),function(){var t=this,e=t._self._c;return e("div",[e("el-dialog",{attrs:{title:"學生列表",visible:t.dialogFormVisible,fullscreen:!1,width:"100%","close-on-press-escape":!1,"close-on-click-modal":!1},on:{"update:visible":function(e){t.dialogFormVisible=e},close:t.close}},[e("el-row",[e("el-col",{attrs:{span:24}},[e("el-form",{attrs:{model:t.form,inline:!0}},[e("el-form-item",{attrs:{label:"班級:",size:"mini"}},[e("el-select",{attrs:{placeholder:""},model:{value:t.form.cls_id,callback:function(e){t.$set(t.form,"cls_id",e)},expression:"form.cls_id"}},t._l(t.clslist,(function(t){return e("el-option",{key:t.cls_id,attrs:{label:t.cls_abr,value:t.cls_id}})})),1)],1),e("el-form-item",{attrs:{label:"學生姓名：",size:"mini"}},[e("el-input",{staticStyle:{width:"200px"},attrs:{placeholder:"輸入關鍵字篩選"},model:{value:t.form.std_name,callback:function(e){t.$set(t.form,"std_name",e)},expression:"form.std_name"}})],1),e("el-form-item",{attrs:{label:"",size:"mini"}},[e("el-button",{attrs:{type:"primary",plain:"",size:"mini"},on:{click:t.query}},[t._v("查詢")])],1)],1)],1)],1),e("el-row",[e("el-col",{attrs:{span:24}},[e("el-table",{staticStyle:{width:"100%"},attrs:{data:t.stulist,height:"50vh",stripe:"","row-style":t.rowState},on:{"header-click":t.headerclick}},[e("el-table-column",{attrs:{type:"index",width:"50"}}),e("el-table-column",{attrs:{prop:"x_status",width:"80","header-align":"center",align:"center",label:"全選"},scopedSlots:t._u([{key:"default",fn:function(a){return[e("el-checkbox",{model:{value:a.row.x_status,callback:function(e){t.$set(a.row,"x_status",e)},expression:"scope.row.x_status"}})]}}])}),t._l(t.render_header,(function(t){return e("el-table-column",{key:t.prop,attrs:{prop:t.prop,label:t.label,width:t.width}})}))],2),e("el-pagination",{attrs:{"page-size":t.pageSize,total:t.total,layout:"total,prev, pager, next,sizes"},on:{"current-change":t.current_change,"size-change":t.size_change}})],1)],1),e("br"),e("br"),e("div",{attrs:{align:"right"}},[e("el-button",{attrs:{type:"primary"},on:{click:t.save}},[t._v("新增諮詢學生")]),e("el-button",{attrs:{type:"primary"},on:{click:t.cancel}},[t._v("關閉視窗")])],1)],1)],1)}),k=[],S={props:{dialog_show:{type:Boolean},filelist:{type:Array},userStatic:{type:Object},parameter:{type:Object},api_interface:{type:Object},data_structure:{type:Object},tea_consult:{type:Object}},data(){return{dialogFormVisible:this.dialog_show,total:0,currentPage:1,pageSize:10,stulist:[],clslist:[],check_list:[],form:{cls_id:"",std_name:"",year_id:"",sms_id:"",emp_id:"",sch_no:"",sRowNun:1,eRowNun:10,x_status:"",cls_id_q:"",std_name_q:"",token:this.$token}}},methods:{rowState(t,e){return{backgroundColor:"#f4f4f5"}},close:function(){this.$emit("get-show",!1)},cancel:function(){this.dialogFormVisible=!1},getshow:function(t){this.isShow=t.show},save:function(){var t=0,e=this;if(e.check_list.length=0,this.stulist.forEach(((a,s)=>{1==a.x_status&&(e.check_list[t++]=a)})),0==e.check_list.length)return e.$message.error("未勾選諮詢學生，請確認!!"),!1;this.$emit("update:dialog_show",!0),this.dialogFormVisible=!1,this.$emit("get-data",e.check_list)},query:function(){let t=this;t.form.year_id=this.parameter.year_id,t.form.sms_id=this.parameter.sms_id,t.form.sch_no=this.parameter.sch_no,t.form.emp_id=this.parameter.emp_id,t.form.token=this.$token;const e=t.api_interface.get_studata;""==t.form.std_name?t.form.std_name_q="%":t.form.std_name_q="%"+t.form.std_name+"%",""==t.form.cls_id?t.form.cls_id_q="%":t.form.cls_id_q=t.form.cls_id,""!=t.form.std_name&&(t.form.sRowNun=1,t.form.eRowNun=10,t.currentPage=1,t.pageSize=10);const a=t.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});t.$http({url:e,method:"get",params:t.form,headers:{SkyGet:t.$token}}).then((e=>{"Y"==e.data.status?(t.stulist=e.data.dataset,t.total=e.data.dataset[0].x_cnt):(t.stulist=[],t.$message.error("查無資料!!"))})).catch((e=>{t.stulist=[],t.$message({message:"系統發生錯誤"+e,type:"error",duration:0,showClose:!0})})).finally((()=>{t.form.std_name="",a.close()}))},headerclick(t,e){"全選"==t.label?(t.label="全不選",this.stulist.forEach(((t,e)=>{0!=t.x_status&&""!=t.x_status||(t.x_status=!0)}))):(t.label="全選",this.stulist.forEach(((t,e)=>{1!=t.x_status&&""!=t.x_status||(t.x_status=!1)})))},current_change(t){var e=this,a="",s="";a=(t-1)*e.pageSize+1,s=t*e.pageSize,e.currentPage=t,e.form.sRowNun=a,e.form.eRowNun=s,e.query()},size_change(t){var e=this,a="",s="";a=(e.currentPage-1)*t+1,s=e.currentPage*t,e.pageSize=t,e.form.sRowNun=a,e.form.eRowNun=s,e.query()}},mounted(){let t=this;const e=`${t.$apiroot}/TeaConsult/clsdata`;t.parameter.sch_no=this.tea_consult.a,t.parameter.year_id=this.tea_consult.b,t.parameter.sms_id=this.tea_consult.c,t.parameter.emp_id=this.tea_consult.d,t.parameter.token=this.$token,t.$http({url:e,method:"get",params:t.parameter,headers:{SkyGet:t.$token}}).then((e=>{"Y"==e.data.status?t.clslist=e.data.dataset:t.clslist=[]})).catch((e=>{t.clslist=[],t.$message({message:"系統發生錯誤"+e,type:"error",duration:0,showClose:!0})})).finally()},computed:{render_header(){let t=this.data_structure.sub_header.filter((t=>"N"===t.hidden)).sort((function(t,e){return t.sort>e.sort?1:-1}));return t}}},x=S,$=(0,m.Z)(x,y,k,!1,null,"21aa0f8e",null),v=$.exports;var D={props:{userStatic:{type:Object},api_interface:{type:Object},parentData:{type:Array},total:{type:Number},complex_key:{type:Object},tea_consult:{type:Object}},data(){return{studata:{sch_no:"",year_id:0,sms_id:0,emp_id:"",ser_id:0,std_no:[]},is_readonly:!1,method_type:"post",check_list:[],currentPage:1,pageSize:10,data_structure:{},dialog_show:!0,isShow:!1,parameter:{},tableData:[],stdarray:[],tag:""}},methods:{getshow:function(t){this.isShow=t.show},getstudata:function(t){this.tableData=this.tableData.concat(t);const e=[],a=new Map;for(const s of this.tableData)a.has(s.b)||(a.set(s.b,!0),e.push(s));this.tableData=e.sort((function(t,e){return t.b>e.b?1:-1})),this.tableData.forEach((function(t,e,a){t.x_status=!1})),this.save()},cellmouseenter(t,e,a,s){},headercellstyle({column:t}){const e=["全選","全不選"];if(e.includes(t.label))return{backgroundColor:"red",color:"#333",cursor:pointer}},rowState(t,e){return{backgroundColor:"#f4f4f5"}},add:function(){this.isShow=!0},del:function(t,e,a,s){let r=this;if("Y"===this.data_structure.delete_rule.rule_flag&&!this.data_structure.delete_rule.rule_check(val,r))return!1;r.$confirm("確定刪除?","Warning",{confirmButtonText:"確定",cancelButtonText:"取消",type:"warning"}).then((()=>{const o=r.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});r.$http({headers:{"Content-Type":"application/json;charset=utf-8",SkyGet:r.$token},url:t,method:"delete",data:e}).then((t=>{if("Y"==t.data.status)if(r.$message.success("刪除成功!!"),"del_batch"==s)for(var e=0;e<a.length;e++)r.tableData.forEach((function(t,s,o){a[e].b==t.b&&r.tableData.splice(s,1)}));else 0==a?r.tableData.splice(0,1):r.tableData.splice(a,1);else r.$message.error("刪除失敗!!")})).catch((t=>{r.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>o.close()))})).catch((()=>{}))},del_batch:function(){var t=0,e=this;let a=new FormData;if(a.append("sch_no",this.tea_consult.a),a.append("year_id",this.tea_consult.b),a.append("sms_id",this.tea_consult.c),a.append("emp_id",this.tea_consult.d),a.append("ser_id",this.tea_consult.e),a.append("token",this.$token),e.check_list.length=0,this.tableData.forEach(((s,r)=>{1==s.x_status&&(s.row=r,e.check_list[t++]=s,a.append("std_no[]",s.b))})),0==e.check_list.length)return e.$message.error("未勾選諮詢學生，請確認!!"),!1;const s=e.api_interface.del_studata;e.del(s,a,e.check_list,"del_batch")},del_single:function(t){let e=this,a=new FormData,s={row:t.$index};e.check_list.length=0,e.check_list[0]=s,a.append("std_no[]",t.row.b),a.append("sch_no",this.tea_consult.a),a.append("year_id",this.tea_consult.b),a.append("sms_id",this.tea_consult.c),a.append("emp_id",this.tea_consult.d),a.append("ser_id",this.tea_consult.e),a.append("token",this.$token);const r=e.api_interface.del_studata;e.del(r,a,t.$index,"del_single")},headerclick(t,e){"全選"!=t.label&&"全不選"!=t.label||("全選"==t.label?(t.label="全不選",this.tableData.forEach(((t,e)=>{0!=t.x_status&&""!=t.x_status||(t.x_status=!0)}))):(t.label="全選",this.tableData.forEach(((t,e)=>{1!=t.x_status&&""!=t.x_status||(t.x_status=!1)}))))},save:function(){let t=this,e=new FormData;this.tableData.forEach((function(t,a,s){e.append("std_no[]",t.b)})),e.append("sch_no",this.tea_consult.a),e.append("year_id",this.tea_consult.b),e.append("sms_id",this.tea_consult.c),e.append("emp_id",this.tea_consult.d),e.append("ser_id",this.tea_consult.e),e.append("token",this.$token);const a=t.$loading({lock:!0,text:"資料處理中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"}),s=t.api_interface.save_stuconsult;t.$http({url:s,method:t.method_type,data:e,headers:{SkyGet:t.$token}}).then((e=>{"Y"==e.data.status?t.$message.success("新增成功!!"):t.$message.error(e.data.message)})).catch((e=>{t.$message({message:"新增失敗:"+e,type:"error",duration:0,showClose:!0})})).finally((()=>a.close()))},close:function(){},getyear:function(t){this.form.year_id=t},getsms:function(t){this.form.sms_id=t},current_change(t){var e=this,a="",s="";a=(t-1)*e.pageSize+1,s=t*e.pageSize,e.currentPage=t,this.get_data(e.complex_key.a,e.complex_key.b,e.complex_key.c,e.complex_key.d,e.complex_key.e,a,s)},size_change(t){var e=this,a="",s="";a=(e.currentPage-1)*t+1,s=e.currentPage*t,e.pageSize=t,this.get_data(e.complex_key.a,e.complex_key.b,e.complex_key.c,e.complex_key.d,e.complex_key.e,a,s)},get_data:function(t,e,a,s,r,o,i){let n=this,l=n.api_interface.get_stuconsult;const c=n.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});n.parameter.emp_id=s,n.parameter.year_id=e,n.parameter.sms_id=a,n.parameter.sRowNun=o,n.parameter.eRowNun=i,n.parameter.sch_no=t,n.parameter.ser_id=r,n.$http({url:l,method:"get",params:n.parameter,headers:{SkyGet:n.$token}}).then((t=>{"Y"==t.data.status?(n.tableData=t.data.dataset,n.total=t.data.dataset[0].x_cnt):(n.tableData=[],n.total=0,n.$message.error("查無資料!!"))})).catch((t=>{n.tableData=[],n.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>c.close()))}},mounted(){},components:{PubStuList:v},computed:{render_header(){let t=this.data_structure.sub_header.filter((t=>"N"===t.hidden)).sort((function(t,e){return t.sort>e.sort?1:-1}));return t}},beforeMount(){switch(this.userStatic.data_structure){case"teaconsult":this.data_structure=d.H_;break;default:}},watch:{parentData:function(t){this.tableData=this.parentData},tea_consult:function(){this.tag=`諮詢日期:${this.tea_consult.f}\n                          諮詢地點:${this.tea_consult.g}\n                          諮詢方式:${this.tea_consult.h}\n                          諮詢主題:${this.tea_consult.i}\n                         `}}},z=D,C=(0,m.Z)(z,b,w,!1,null,null,null),N=C.exports,P="",R={name:"TeaConsultView",props:{userStatic:{type:Object},api_interface:{type:Object}},data:function(){return{activeName:"first",pagetotal1:0,pagetotal2:0,currentPage:1,pageSize:10,parameter:{},tableData:[],stuData:[],complex_key:{},tea_consult:{}}},components:{PubQuery:o.Z,TeaConsultGrid:f,TeaConsultStu:N},methods:{getstudata:function(t){this.tea_consult=t,this.get_studata(t.a,t.b,t.c,t.d,t.e,1,10)},handleClick:function(){},getcondition:function(t){var e=this;e.year=t.year,e.sms=t.sms,P=e.api_interface.get_data,this.get_data(P,t.year,t.sms,e.currentPage,e.pageSize)},get_data:function(t,e,a,s,r){let o=this;const i=o.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});o.parameter.emp_id="",o.parameter.year_id=e,o.parameter.sms_id=a,o.parameter.sRowNun=s,o.parameter.eRowNun=r,o.parameter.sch_no="",o.parameter.token=o.$token,o.$http({url:t,method:"get",params:o.parameter,headers:{SkyGet:o.$token}}).then((t=>{"Y"==t.data.status?(o.tableData=t.data.dataset,o.pagetotal1=t.data.total,o.complex_key=t.data.dataset[0],this.tea_consult=t.data.dataset[0],o.get_studata(t.data.dataset[0].a,t.data.dataset[0].b,t.data.dataset[0].c,t.data.dataset[0].d,t.data.dataset[0].e,s,r)):(o.tableData=[],o.$message.error("查無資料，請確認"))})).catch((t=>{o.tableData=[],o.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>i.close()))},get_studata:function(t,e,a,s,r,o,i){let n=this;P=n.api_interface.get_stuconsult,n.parameter.emp_id=s,n.parameter.year_id=e,n.parameter.sms_id=a,n.parameter.sRowNun=o,n.parameter.eRowNun=i,n.parameter.sch_no=t,n.parameter.ser_id=r,n.$http({url:P,method:"get",params:n.parameter,headers:{SkyGet:n.$token}}).then((t=>{"Y"==t.data.status?(n.stuData=t.data.dataset,n.pagetotal2=t.data.dataset[0].x_cnt):(n.stuData=[],n.pagetotal2=0)})).catch((t=>{n.tableData=[],n.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally()}},mounted:function(){}},Y=R,j=(0,m.Z)(Y,s,r,!1,null,null,null),O=j.exports}}]);