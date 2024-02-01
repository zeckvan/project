//課程諮詢
const consult_cls =
{
    tea_hedaer :
    [
        {label:'教師員編',width:'',prop:'emp_id',col:'emp_id',parameter:'Y',hidden:'N',sort:1,defult:''},
        {label:'教師姓名',width:'',prop:'emp_name',col:'emp_name',parameter:'N',hidden:'N',sort:2,defult:''},
        {label:'教郵e-mail',width:'',prop:'emp_email',col:'emp_email',parameter:'N',hidden:'N',sort:3,defult:''},
    ],
    cls_header:
    [
      {label:'學年',width:'50',prop:'year_id',col:'year_id',parameter:'N',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'sms_id',col:'sms_id',parameter:'N',hidden:'N',sort:2,defult:''},
      {label:'班級名稱',width:'110',prop:'cls_abr',col:'cls_abr',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'班級代碼',width:'',prop:'cls_code',col:'cls_code',parameter:'N',hidden:'Y',sort:4,defult:''},
      {label:'',width:'',prop:'deg_id',col:'deg_id',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'',width:'',prop:'dep_id',col:'dep_id',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'',width:'',prop:'bra_id',col:'bra_id',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'',width:'',prop:'grd_id',col:'grd_id',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'',width:'',prop:'cls_id',col:'cls_id',parameter:'N',hidden:'Y',sort:0,defult:''},    
    ],
    consult_header:
    [
      {label:'學年',width:'',prop:'a',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'',prop:'b',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'班級名稱',width:'',prop:'c',col:'cls_abr',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'班級代碼',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:4,defult:''},
    ],    
    delete_rule:{},
    save_rule:{}
}

const System_SetUp = 
{
  header:
  [
   // {label:'',width:'50',prop:'',col:'',parameter:'N',hidden:'N',sort:1,defult:'',align:"center",slot:false,type:"default",index:"Y"},    
    {label:'項目',width:'',prop:'name',col:'name',parameter:'Y',hidden:'N',sort:2,defult:'',align:"left",slot:false,type:"default",index:"N"},
    {label:'數量',width:'',prop:'value',col:'value',parameter:'Y',hidden:'N',sort:3,defult:'',align:"center",slot:true,type:"input",index:"N"},
    {label:'單位',width:'',prop:'unit',col:'unit',parameter:'Y',hidden:'N',sort:4,defult:'',align:"center",slot:true,type:"input",index:"N"},
    {label:'說明',width:'',prop:'memo',col:'memo',parameter:'Y',hidden:'N',sort:5,defult:'',align:"center",slot:true,type:"input",index:"N"},
  ],
  delete_rule:{},
  save_rule:{}
}

const Operate_SetUp = 
{
  header:
  [  
    {label:'學年',width:'100',prop:'year_id',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:'',align:"left",slot:false,type:"input",index:"N"},
    {label:'學期',width:'100',prop:'sms_id',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:'',align:"center",slot:false,type:"input",index:"N"},
    {label:'項目',width:'',prop:'type_id',col:'type_id',parameter:'Y',hidden:'N',sort:3,defult:'',align:"center",slot:true,type:"select",index:"N"},
    {label:'開放日期',width:'',prop:'startdate',col:'startdate',parameter:'Y',hidden:'N',sort:4,defult:'',align:"center",slot:true,type:"date",index:"N"},
    {label:'結束日期',width:'',prop:'enddate',col:'enddate',parameter:'Y',hidden:'N',sort:5,defult:'',align:"center",slot:true,type:"date",index:"N"},
  ],
  delete_rule:{},
  save_rule:{}
}

export
{
  consult_cls,System_SetUp,Operate_SetUp
}
