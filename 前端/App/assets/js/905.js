"use strict";(self["webpackChunkportfolio"]=self["webpackChunkportfolio"]||[]).push([[905],{4751:function(t,e,a){a.r(e),a.d(e,{default:function(){return x}});var r=function(){var t=this,e=t._self._c;return e("div",[e("div",{attrs:{align:"left"}},[e("v-pubquery",{on:{"get-condition":t.getcondition}})],1),e("div",{attrs:{align:"left"}},["Y"==t.operate_state.open_yn?e("div",[e("el-button",{attrs:{type:"primary"},on:{click:t.centraldb_batch}},[t._v("確認存檔")])],1):e("div",[e("el-alert",{attrs:{closable:!1,title:"系統開放勾選時間："+t.operate_state.startdate+"~"+t.operate_state.enddate,type:"error"}})],1)]),e("div",{staticClass:"container"},[e("div",{staticClass:"item-right",staticStyle:{width:"25%",height:"100%"}},[e("el-table",{attrs:{data:t.leftList,height:"70vh",stripe:"","row-class-name":t.rowclassname,"cell-style":t.cellstyle,"row-style":t.leftrowState,"cell-class-name":t.classChecker},on:{"row-click":t.rowclick,"cell-click":t.cellclick}},[e("el-table-column",{attrs:{type:"index",width:"50"}}),e("el-table-column",{attrs:{prop:"diverseid",label:"多元學習項目"}})],1)],1),e("div",{staticClass:"item-left",staticStyle:{width:"75%",height:"100%"}},[e("el-table",{attrs:{data:t.tableData,height:"70vh",stripe:"","row-style":t.rowState},on:{"header-click":t.headerclick}},[e("el-table-column",{attrs:{type:"index",width:"50"}}),e("el-table-column",{staticStyle:{cursor:"pointer"},attrs:{prop:"x_status",width:"80","header-align":"center",align:"center",label:"全選"},scopedSlots:t._u([{key:"default",fn:function(a){return[e("el-checkbox",{model:{value:a.row.x_status,callback:function(e){t.$set(a.row,"x_status",e)},expression:"scope.row.x_status"}})]}}])}),t._l(t.render_header,(function(t){return e("el-table-column",{key:t.prop,attrs:{prop:t.prop,label:t.label,width:t.width,align:"center"}})})),t.data_structure.query_data?e("el-table-column",{attrs:{align:"center",fixed:"right",label:"檢視資料",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("i",{staticClass:"el-icon-search",staticStyle:{cursor:"pointer"},on:{click:function(e){return t.edit_data(a)}}})]}}],null,!1,352033032)}):t._e(),t.data_structure.download_file?e("el-table-column",{attrs:{align:"center",fixed:"right",label:"查看檔案",width:""},scopedSlots:t._u([{key:"default",fn:function(a){return[e("el-badge",{staticClass:"item",staticStyle:{cursor:"pointer"},attrs:{value:a.row.x_cnt,type:"info"},nativeOn:{click:function(e){return t.file_data(a)}}})]}}],null,!1,3552961885)}):t._e()],2),e("el-pagination",{attrs:{"page-size":t.pageSize,total:t.total,layout:"total,prev, pager, next,sizes"},on:{"current-change":t.current_change,"size-change":t.size_change}}),t.isShow?e(t.content,{tag:"component",attrs:{dialog_show:t.dialog_show,edit_model:t.edit_model,parameter:t.parameter,data_structure:t.data_structure,userStatic:t.userStatic,api_interface:t.api_interface,formSaveCheck:t.formSaveCheck},on:{"update:dialog_show":function(e){t.dialog_show=e},"get-show":t.getshow}}):t._e(),t.isShow2?e("v-stufilelist",{attrs:{dialog_show:t.dialog_show2,filelist:t.filelist,userStatic:t.userStatic,parameter:t.parameter,api_interface:t.api_interface},on:{"update:dialog_show":function(e){t.dialog_show2=e},"get-show":t.getshow2}}):t._e(),t.isShow3?e("v-fileupload",{attrs:{dialog_show:t.dialog_show3,filelist:t.filelist,userStatic:t.userStatic,api_interface:t.api_interface,parameter:t.parameter,data_structure:t.data_structure,rowdata:t.rowdata},on:{"update:dialog_show":function(e){t.dialog_show3=e},"get-show":t.getshow3}}):t._e()],1)])])},s=[],o=a(2081),i=a(7020),n=a(6157),c=a(8227),l=a(4779),u=a(5974),d=a(5714),h=a(7296),p=a(5922),_=a(7277),f=a(9405),m=a(2167),g=a(5705),w=a(9005),k=a(8304),b="",y={props:{},data(){return{opno:"",formSaveCheck:"studiversecheck",userStatic:{},api_interface:{},content:"",data_structure:{},tableData:[],isShow:!1,isShow2:!1,isShow3:!1,dialog_show:!0,dialog_show2:!0,dialog_show3:!0,total:0,currentPage:1,pageSize:10,year:"",sms:"",edit_model:"",parameter:{},filelist:[],rowdata:{},check_list:[],leftList:[{diverseid:"幹部經歷紀錄",width:"",prop:"",col:"stucadre",parameter:"N",hidden:"N",sort:1,defult:"",slot:!1},{diverseid:"競賽參與紀錄",width:"",prop:"",col:"stucompetition",parameter:"N",hidden:"N",sort:2,defult:"",slot:!1},{diverseid:"檢定證照紀錄",width:"",prop:"",col:"stulicense",parameter:"N",hidden:"N",sort:3,defult:"",slot:!1},{diverseid:"檢視志工服務紀錄",width:"",prop:"",col:"stuvolunteer",parameter:"N",hidden:"N",sort:4,defult:"",slot:!1},{diverseid:"作品成果紀錄",width:"",prop:"",col:"sturesult",parameter:"N",hidden:"N",sort:5,defult:"",slot:!1},{diverseid:"其他活動紀錄",width:"",prop:"",col:"stuother",parameter:"N",hidden:"N",sort:6,defult:"",slot:!1},{diverseid:"彈性學習紀錄",width:"",prop:"",col:"stustudyf",parameter:"N",hidden:"N",sort:7,defult:"",slot:!1},{diverseid:"職場學習紀錄",width:"",prop:"",col:"stuworkplace",parameter:"N",hidden:"N",sort:8,defult:"",slot:!1},{diverseid:"大學及技專校院先修課程紀錄",width:"",prop:"",col:"stucollege",parameter:"N",hidden:"N",sort:9,defult:"",slot:!1},{diverseid:"團體活動時間紀錄",width:"",prop:"",col:"stugroup",parameter:"N",hidden:"N",sort:10,defult:"",slot:!1}],queryform:{year_id:"",sms_id:"",cls_id:"",std_no:"",std_name:"",emp_id:"",sRowNun:1,eRowNun:10},yearlist:[],operate_state:{open_yn:"",startdate:"",enddate:""},color:"#606266",inlineClass:"color:red",getIndex:""}},methods:{query:function(){},yearChange:function(){},getyearlist:function(){let t=this;const e=`${t.$apiroot}/s90yearinfo`;return new Promise((function(a,r){t.$http({url:e,method:"get",headers:{SkyGet:t.$token}}).then((e=>{"Y"==e.data.status?(t.yearlist=e.data.dataset,t.queryform.year_id=e.data.dataset[0].year_id.toString()):(t.queryform.year_id="",t.yearlist=[])})).catch((e=>{t.$message.error("呼叫後端【s90yearinfo】發生錯誤,"+e)})).finally()}))},rowclassname:function(t,e){},cellstyle:function(t,e,a,r){},classChecker({row:t,column:e}){},cellclick:function(t,e,a,r){},rowclick:function(t,e,a){this.getIndex=t.sort;let r=this;switch(r.opno=t.col,t.col){case"studiversecheck":this.data_structure=k.y3;break;case"stucadre":this.data_structure=k.CX;break;case"stustudyf":this.data_structure=k.xQ;break;case"stugroup":this.data_structure=k.L1;break;case"stucollege":this.data_structure=k.Xk;break;case"stuworkplace":this.data_structure=k.Hj;break;case"stuother":this.data_structure=k.uD;break;case"sturesult":this.data_structure=k.RM;break;case"stuvolunteer":this.data_structure=k.vo;break;case"stulicense":this.data_structure=k.vL;break;case"stucompetition":this.data_structure=k.zz;break;case"stuconsult_query":this.data_structure=k.cL;break;case"stuattestation":this.data_structure=k.Hc;break;case"stuattestationconfirm":this.data_structure=k.Cd;break;case"stuattestationcentraldb":this.data_structure=k.Eh;break;default:}this.$router.options.routes.find((function(e,a,s){e.name==t.col&&(r.api_interface=e.props.api_interface,r.userStatic=e.props.userStatic,r.userStatic.file_delete=!1,r.content=e.props.userStatic.form_component)})),r.tableData=[]},centraldb_batch:function(){var t=0;let e=this,a=new FormData;e.check_list.length=0,this.tableData.forEach(((a,r)=>{e.check_list[t++]=a})),e.check_list.forEach((function(t,r,s){"stucadre"==e.opno?a.append("complex_array[]",t.a+"_"+t.c+"_"+t.d+"_"+t.b+"_"+t.e+"_"+t.h+"@"+t.x_status):a.append("complex_array[]",t.a+"_"+t.b+"_"+t.c+"_"+t.d+"_"+t.e+"@"+t.x_status)})),a.append("token",e.$token);const r=e.$loading({lock:!0,text:"資料處理中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"}),s=e.api_interface.save_centraldb;e.$http({url:s,method:"put",data:a,headers:{SkyGet:e.$token}}).then((t=>{if("Y"==t.data.status){e.$message.success("存檔成功!!");var a={year:e.year,sms:e.sms};e.getcondition(a)}else e.$message.error(t.data.message)})).catch((t=>{e.$message({message:"存檔失敗:"+t,type:"error",duration:0,showClose:!0})})).finally((()=>r.close()))},confirm_batch:function(){var t=0;let e=this,a=new FormData;if(e.check_list.length=0,this.tableData.forEach(((a,r)=>{1==a.x_status&&(e.check_list[t++]=a)})),0==e.check_list.length)return e.$message.error("未勾選學習成果認証，請確認!!"),!1;e.check_list.forEach((function(t,e,r){a.append("complex_key[]",t.a+"_"+t.b+"_"+t.c+"_"+t.d+"_"+t.e+"_"+t.f+"_"+t.g+"_"+t.h)})),e.$confirm("確定送出學習成果認証?","Warning",{confirmButtonText:"確定",cancelButtonText:"取消",type:"warning"}).then((()=>{const t=e.$loading({lock:!0,text:"資料處理中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"}),r=e.api_interface.save_form;e.$http({url:r,method:"put",data:a,headers:{SkyGet:e.$token}}).then((t=>{if("Y"==t.data.status){e.$message.success("送出成功!!");var a={year:e.year,sms:e.sms};e.getcondition(a)}else e.$message.error(t.data.message)})).catch((t=>{e.$message({message:"送出失敗:"+t,type:"error",duration:0,showClose:!0})})).finally((()=>t.close()))})).catch((()=>{}))},cellmouseenter(t,e,a,r){},headerclick(t,e){"全選"!=t.label&&"全不選"!=t.label||("全選"==t.label?(t.label="全不選",this.tableData.forEach(((t,e)=>{0!=t.x_status&&""!=t.x_status||(t.x_status=!0)}))):(t.label="全選",this.tableData.forEach(((t,e)=>{1!=t.x_status&&""!=t.x_status||(t.x_status=!1)}))))},send:function(t){let e=this,a=new FormData;if(0==t.row.x_cnt)return e.$message.warning("請先上傳檔案後，再執行送出認証。"),!1;this.data_structure.header.forEach((function(e,r,s){a.append(e.col,t.row[e.prop])})),e.parameter.token=e.$token,e.$confirm("確定送出?一但送出後該項目無法修正!!","Warning",{confirmButtonText:"確定",cancelButtonText:"取消",type:"warning"}).then((()=>{const t=e.$loading({lock:!0,text:"資料處理中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"}),r=e.api_interface.save_form;e.$http({url:r,method:"put",data:a,headers:{SkyGet:e.$token}}).then((t=>{if("Y"==t.data.status){e.$message.success("送出成功!!");var a={year:e.year,sms:e.sms};e.getcondition(a)}else e.$message.error(t.data.message)})).catch((t=>{e.$message({message:"送出失敗:"+t,type:"error",duration:0,showClose:!0})})).finally((()=>t.close()))})).catch((()=>{}))},leftrowState(t,e){return this.getIndex==t.row.sort?{backgroundColor:"#f4f4f5",cursor:"pointer",color:"blue","font-weight":"bold"}:{backgroundColor:"#f4f4f5",cursor:"pointer",color:"#606266"}},rowState(t,e){return{backgroundColor:"#f4f4f5",cursor:"pointer"}},add:function(){this.edit_model="add",this.isShow=!0},edit_data:function(t){let e=this;this.data_structure.header.forEach((function(a,r,s){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])})),this.edit_model="edit",this.isShow=!0},file_data:function(t){let e=this,a="";b=e.api_interface.file_data;const r=e.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});e.data_structure.header.forEach((function(a,r,s){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])})),e.parameter["class_name"]=e.userStatic.interface,"StuAttestation"==e.userStatic.interface&&("N"==t.row.n||"Y"==t.row.n?(e.userStatic.file_delete=!1,e.parameter["deleteFile"]="N"):(e.userStatic.file_delete=!0,e.parameter["deleteFile"]="Y")),a="StuAttestation"==e.userStatic.interface?`${t.row.a}_${t.row.b}_${t.row.c}_${t.row.d}_${t.row.e}_${t.row.f}_${t.row.g}_${t.row.h}_0`:"StudCadre"==e.userStatic.interface?`${t.row.a}_${t.row.c}_${t.row.d}_${t.row.b}_${t.row.e}_${t.row.h}`:`${t.row.a}_${t.row.b}_${t.row.c}_${t.row.d}_${t.row.e}`,e.parameter["complex_key"]=a,e.$http({url:b,method:"get",params:e.parameter,headers:{SkyGet:e.$token}}).then((t=>{"Y"==t.data.status?e.filelist=t.data.dataset:(e.filelist=[],e.$message.error("查無資料，請確認"))})).catch((t=>{e.tableData=[],e.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>r.close())),this.isShow2=!0},file_upload:function(t){let e=this;if("StuAttestation"==e.userStatic.interface&&("N"==t.row.n||"Y"==t.row.n))return e.$message.warning("該項目狀態為「認証中」或「認証成功」，無法執行檔案上傳功能。"),!1;b=e.api_interface.file_upload,e.rowdata=t,e.data_structure.header.forEach((function(a,r,s){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop])})),this.isShow3=!0},del_data:function(t){let e=this;if("Y"===this.data_structure.delete_rule.rule_flag&&!this.data_structure.delete_rule.rule_check(t,e))return!1;e.$confirm("確定刪除?","Warning",{confirmButtonText:"確定",cancelButtonText:"取消",type:"warning"}).then((()=>{const a=e.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});let r=new FormData;e.data_structure.header.forEach((function(a,s,o){"Y"==a.parameter&&(e.parameter[a.col]=t.row[a.prop],r.append(a.col,t.row[a.prop]))})),r.append("class_name",e.userStatic.interface),b=e.api_interface.del_data,e.$http({headers:{"Content-Type":"application/json;charset=utf-8",SkyGet:e.$token},url:b,method:"delete",data:r}).then((a=>{"Y"==a.data.status?(e.$message.success("刪除成功!!"),e.tableData.splice(t.$index,1)):e.$message.error("刪除失敗!!")})).catch((t=>{e.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>a.close()))})).catch((()=>{}))},getshow:function(t){this.isShow=t.show},getshow2:function(t){this.isShow2=t.show},getshow3:function(t){this.isShow3=t.show},getcondition:function(t){var e=this;e.year=t.year,e.sms=t.sms,e.tableData=[],e.total=0,b=e.api_interface.get_data,this.get_data(b,t.year,t.sms,e.currentPage,e.pageSize)},current_change(t){var e=this,a="",r="";a=(t-1)*e.pageSize+1,r=t*e.pageSize,b=e.api_interface.get_data,this.get_data(b,e.year,e.sms,a,r)},size_change(t){var e=this,a="",r="";a=(e.currentPage-1)*t+1,r=e.currentPage*t,b=e.api_interface.g,b=e.api_interface.get_data,this.get_data(b,e.year,e.sms,a,r)},get_data:function(t,e,a,r,s){let o=this;const i=o.$loading({lock:!0,text:"資料讀取中，請稍後。",spinner:"el-icon-loading",background:"rgba(0, 0, 0, 0.7)"});o.parameter.emp_id="",o.parameter.std_no="",o.parameter.token=o.$token,o.parameter.year_id=e,o.parameter.sms_id=a,o.parameter.sRowNun=r,o.parameter.eRowNun=s,o.parameter.sch_no="",o.parameter.is_sys="2",o.$http({url:t,method:"get",params:o.parameter,headers:{SkyGet:o.$token}}).then((t=>{"Y"==t.data.status?("stucadre"==o.opno?o.tableData=t.data.dataset.filter((function(t,e,a){return"2"==t.j})):o.tableData=t.data.dataset,o.tableData.forEach((function(t,e,a){"Y"==t.zz?t.x_status=!0:t.x_status=!1})),o.total=t.data.dataset[0].x_total):(o.tableData=[],o.total=0,o.$message.error("查無資料，請確認"))})).catch((t=>{o.tableData=[],o.$message({message:"系統發生錯誤"+t,type:"error",duration:0,showClose:!0})})).finally((()=>i.close()))}},components:{"v-stucadreform":i.Z,"v-stustudyform":n.Z,"v-pubquery":g.Z,"v-stufilelist":o.Z,"v-fileupload":w.Z,"v-stugroupform":c.Z,"v-stucollegeform":l.Z,"v-stuworkplaceform":u.Z,"v-stuotherform":d.Z,"v-sturesultform":h.Z,"v-stuvolunteerform":p.Z,"v-stulicenseform":_.Z,"v-stucompetitionform":f.Z,"v-stuconsultQueryform":m.Z},beforeDestroy(){},mounted(){},beforeMount(){let t=this;this.opno="stucadre",this.data_structure=k.CX,this.data_structure.file_delete=!0,this.$router.options.routes.find((function(e,a,r){"stucadre"==e.name&&(t.api_interface=e.props.api_interface,t.userStatic=e.props.userStatic,t.content=e.props.userStatic.form_component)})),t.tableData=[],this.$router.options.routes.find((function(e,a,r){if("studiversecheck"==e.name){b=e.props.api_interface.get_OpOpenYN;let a={year_id:111,sms_id:0,grade_id:"1",type_id:"04",token:t.$token};t.operate_state=k.an(t,b,a,t.$token)}}))},computed:{render_header(){let t=this.data_structure.header.filter((t=>"N"===t.hidden)).sort((function(t,e){return t.sort>e.sort?1:-1}));return t}},filters:{timeString(t,e){return moment(t).format(e||"YYYY-MM-DD, HH:mm:ss")}}},$=y,S=a(1001),v=(0,S.Z)($,r,s,!1,null,"702cfb58",null),x=v.exports}}]);