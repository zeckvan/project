import * as adminAPI from  '@/apis/adminApi.js' 

//幹部經歷
const stu_cadre =
{
    header :
    [
        {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
        {label:'學號',width:'',prop:'b',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
        {label:'年度',width:'50',prop:'c',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
        {label:'學期',width:'50',prop:'d',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
        {label:'單位名稱',width:'',prop:'e',col:'unit_name',parameter:'N',hidden:'N',sort:3,defult:''},
        {label:'開始日期',width:'',prop:'f',col:'startdate',parameter:'N',hidden:'N',sort:5,defult:''},
        {label:'結束日期',width:'',prop:'g',col:'enddate',parameter:'N',hidden:'N',sort:6,defult:''},
        {label:'擔任職務',width:'',prop:'h',col:'position_name',parameter:'N',hidden:'Y',sort:0,defult:''},
        {label:'幹部等級',width:'',prop:'i',col:'type_id',parameter:'N',hidden:'N',sort:4,defult:''},
        {label:'資料來源',width:'',prop:'j',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
        {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true}, 
        {label:'序號',width:'',prop:'k',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},      
    ],
    delete_rule:
    {
        rule_flag:"Y",
        rule_message:"此資料為校務系統轉入無法刪除，請確認!!",
        rule_check:function(val,_self){
           if(val.row.j === "1"){
                _self.$notify.error({
                    title: 'Error',
                    message: '此資料為校務系統轉入無法刪除，請確認!!'
                    });
              return false
           }
           return true
        }
    },
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,  
    add_data:true        
}
//彈性學習時間
const stu_study_f =
{
  header :
  [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'類別',width:'',prop:'f',col:'type_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'開設名稱',width:'',prop:'g',col:'open_name',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'開設單位',width:'',prop:'h',col:'open_unit',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'每週節數',width:'',prop:'i',col:'hours',parameter:'Y',hidden:'N',sort:5,defult:''},
      {label:'開設週數',width:'',prop:'j',col:'weeks',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'內容簡述',width:'',prop:'k',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'l',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},         
  ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,   
    add_data:true     
}
//團體活動時間記錄
const stu_group =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'時間類別',width:'',prop:'f',col:'time_id',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'辦理單位',width:'',prop:'g',col:'unit_name',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'內容名稱',width:'',prop:'h',col:'group_content',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'開始時間',width:'',prop:'i',col:'startdate',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'結束時間',width:'',prop:'j',col:'enddate',parameter:'N',hidden:'N',sort:7,defult:''},
      {label:'時數',width:'',prop:'k',col:'hours',parameter:'N',hidden:'N',sort:8,defult:''},
      {label:'內容簡述',width:'',prop:'l',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'m',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,  
    add_data:true      
}
//大專先修課程
const stu_college =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'計畫專案',width:'',prop:'f',col:'project_name',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'開設單位',width:'',prop:'g',col:'unit_name',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'課程名稱',width:'',prop:'h',col:'course_name',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'開始時間',width:'',prop:'i',col:'startdate',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'結束時間',width:'',prop:'j',col:'enddate',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'學分數',width:'',prop:'k',col:'credit',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'總時數',width:'',prop:'l',col:'hours',parameter:'N',hidden:'N',sort:7,defult:''},
      {label:'內容簡述',width:'',prop:'m',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'n',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},         
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,   
    add_data:true     
}
//職場學習
const stu_workplace =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'類別',width:'',prop:'f',col:'type_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'單位',width:'',prop:'g',col:'unit_name',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'職稱',width:'',prop:'h',col:'type_title',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'開始時間',width:'',prop:'i',col:'startdate',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'結束時間',width:'',prop:'j',col:'enddate',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'時數',width:'',prop:'k',col:'hours',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'內容簡述',width:'',prop:'l',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'m',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},         
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,   
    add_data:true      
}
//其它活動紀錄
const stu_other =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'名稱',width:'',prop:'f',col:'other_name',parameter:'Y',hidden:'N',sort:3,defult:''},
      {label:'主辦單位',width:'',prop:'g',col:'unit_name',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'開始時間',width:'',prop:'h',col:'startdate',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'結束時間',width:'',prop:'i',col:'enddate',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'時數',width:'',prop:'j',col:'hours',parameter:'N',hidden:'N',sort:7,defult:''},
      {label:'內容簡述',width:'',prop:'k',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'l',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,  
    add_data:true        
}
//作品成果紀錄
const stu_result =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'名稱',width:'',prop:'f',col:'result_name',parameter:'Y',hidden:'N',sort:3,defult:''},
      {label:'日期',width:'',prop:'g',col:'result_date',parameter:'Y',hidden:'N',sort:4,defult:''},
      {label:'內容簡述',width:'',prop:'h',col:'content',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'i',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,   
    add_data:true      
}
//志工服務
const stu_volunteer =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'服務名稱',width:'',prop:'f',col:'volunteer_name',parameter:'Y',hidden:'N',sort:3,defult:''},
      {label:'服務單位',width:'',prop:'g',col:'volunteer_unit',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'開始時間',width:'',prop:'h',col:'startdate',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'結束時間',width:'',prop:'i',col:'enddate',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'時數',width:'',prop:'j',col:'hours',parameter:'N',hidden:'N',sort:7,defult:''},
      {label:'內容簡述',width:'',prop:'k',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'l',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,   
    add_data:true     
}
//檢定証照
const stu_license =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'證照代碼',width:'',prop:'f',col:'license_id',parameter:'Y',hidden:'N',sort:3,defult:''},
      {label:'證照備註',width:'',prop:'g',col:'license_memo',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'分數',width:'',prop:'h',col:'license_grade',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'分項結果',width:'',prop:'i',col:'license_result',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'取得證照日期',width:'',prop:'j',col:'license_date',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'證照字號',width:'',prop:'k',col:'license_doc',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'檢定組別',width:'',prop:'l',col:'license_group',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'內容簡述',width:'',prop:'m',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'n',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,    
    add_data:true    
}
//競賽參與
const stu_competition =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'學號',width:'',prop:'d',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'競賽名稱',width:'',prop:'f',col:'competition_name',parameter:'Y',hidden:'N',sort:3,defult:''},
      {label:'競賽項目',width:'',prop:'g',col:'competition_item',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'競賽領域',width:'',prop:'h',col:'competition_area',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'競賽等級',width:'',prop:'i',col:'competition_grade',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'獎項',width:'',prop:'j',col:'competition_result',parameter:'N',hidden:'N',sort:5,defult:''},
      {label:'結果公布日期',width:'',prop:'k',col:'competition_date',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'參與類型',width:'',prop:'l',col:'competition_type',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'內容簡述',width:'',prop:'m',col:'content',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'資料來源',width:'',prop:'n',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'多元學習成果勾選確認',width:'',prop:'zz',col:'check_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},               
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:true,
    upload_file:true,
    download_file:true,     
    add_data:true
}
//學生諮詢課程查詢
const consult_query =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:''},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'教師',width:'',prop:'d',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'序號',width:'',prop:'e',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:''},
      {label:'諮詢日期',width:'',prop:'f',col:'consult_date',parameter:'N',hidden:'N',sort:3,defult:''},
      {label:'諮詢地點',width:'',prop:'g',col:'consult_area',parameter:'Y',hidden:'N',sort:5,defult:''},
      {label:'諮詢方式',width:'',prop:'h',col:'consult_type',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'諮詢主題',width:'',prop:'i',col:'consult_subject',parameter:'N',hidden:'N',sort:6,defult:''},
      {label:'諮詢內容',width:'',prop:'j',col:'consult_content',parameter:'Y',hidden:'N',sort:7,defult:''},
      {label:'資料來源',width:'',prop:'k',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:''},
      {label:'人數',width:'',prop:'x_stucnt',col:'x_stucnt',parameter:'N',hidden:'N',sort:4,defult:''},
      {label:'諮詢教師',width:'',prop:'emp_name',col:'emp_name',parameter:'N',hidden:'Y',sort:0,defult:''},
      
    ],
    delete_rule:{},
    save_rule:{},
    query_data:true,
    delete_data:false,
    upload_file:false,
    download_file:true,    
    add_data:false
}

//上傳課程學習成果
const stu_attestation =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:'',slot:false},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:'',slot:false},
	    {label:'班級',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目代碼',width:'',prop:'e',col:'sub_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'分組',width:'',prop:'f',col:'src_dup',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'教師代碼',width:'',prop:'g',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'學生',width:'',prop:'h',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'序號',width:'',prop:'i',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'學分數',width:'70',prop:'j',col:'credit',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
      {label:'修習方式',width:'',prop:'t',col:'scr_study',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
      {label:'送出認證',width:'',prop:'l',col:'attestation_send',parameter:'N',hidden:'N',sort:8,defult:'',slot:true},
      {label:'認證日期',width:'100',prop:'m',col:'attestation_date',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
      {label:'認證狀態',width:'100',prop:'n',col:'attestation_status',parameter:'N',hidden:'N',sort:10,defult:'',slot:true},
      {label:'資料來源',width:'',prop:'o',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'備註',width:'',prop:'p',col:'content',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目名稱',width:'',prop:'q',col:'sub_name',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
	    {label:'教師名稱',width:'',prop:'r',col:'emp_name',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
	    {label:'檔案名稱',width:'',prop:'s',col:'file_name',parameter:'N',hidden:'N',sort:7,defult:'',slot:false}, 
      {label:'未通過原因',width:'',prop:'w',col:'attestation_reason',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'認證發佈',width:'',prop:'x',col:'attestation_release',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'可上傳檔案數',width:'',prop:'x_file_cnt',col:'x_file_cnt',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:true,
    download_file:true,    
    add_data:false,
    course_select:true
}
//上傳課程學習成果
const stu_attestation_course =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:'',slot:false},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:'',slot:false},
	    {label:'班級',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目代碼',width:'',prop:'e',col:'sub_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'分組',width:'',prop:'f',col:'src_dup',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'教師代碼',width:'',prop:'g',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'學生',width:'',prop:'h',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'序號',width:'',prop:'i',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'學分數',width:'70',prop:'j',col:'credit',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
      {label:'修習方式',width:'',prop:'t',col:'scr_study',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
      {label:'送出認證',width:'',prop:'l',col:'attestation_send',parameter:'N',hidden:'Y',sort:8,defult:'',slot:true},
      {label:'認證日期',width:'100',prop:'m',col:'attestation_date',parameter:'N',hidden:'Y',sort:9,defult:'',slot:false},
      {label:'認證狀態',width:'100',prop:'n',col:'attestation_status',parameter:'N',hidden:'Y',sort:10,defult:'',slot:true},
      {label:'資料來源',width:'',prop:'o',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'備註',width:'',prop:'p',col:'content',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目名稱',width:'',prop:'q',col:'sub_name',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
	    {label:'教師名稱',width:'',prop:'r',col:'emp_name',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
	    {label:'檔案名稱',width:'',prop:'s',col:'file_name',parameter:'N',hidden:'Y',sort:7,defult:'',slot:false}, 
      {label:'未通過原因',width:'',prop:'w',col:'attestation_reason',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'認證發佈',width:'',prop:'x',col:'attestation_release',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'可上傳檔案數',width:'',prop:'x_file_cnt',col:'x_file_cnt',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      
    ],
    rightheader:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'Y',sort:1,defult:'',slot:false},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'Y',sort:2,defult:'',slot:false},
	    {label:'班級',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目代碼',width:'',prop:'e',col:'sub_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'分組',width:'',prop:'f',col:'src_dup',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'教師代碼',width:'',prop:'g',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'學生',width:'',prop:'h',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'序號',width:'',prop:'i',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'學分數',width:'70',prop:'j',col:'credit',parameter:'N',hidden:'Y',sort:4,defult:'',slot:false},
      {label:'修習方式',width:'',prop:'t',col:'scr_study',parameter:'N',hidden:'Y',sort:5,defult:'',slot:false},
      {label:'送出認證',width:'',prop:'l',col:'attestation_send',parameter:'N',hidden:'N',sort:8,defult:'',slot:true},
      {label:'認證日期',width:'100',prop:'m',col:'attestation_date',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
      {label:'認證狀態',width:'100',prop:'n',col:'attestation_status',parameter:'N',hidden:'N',sort:10,defult:'',slot:true},
      {label:'資料來源',width:'',prop:'o',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'備註',width:'',prop:'p',col:'content',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目名稱',width:'',prop:'q',col:'sub_name',parameter:'N',hidden:'Y',sort:3,defult:'',slot:false},
	    {label:'教師名稱',width:'',prop:'r',col:'emp_name',parameter:'N',hidden:'Y',sort:6,defult:'',slot:false},
	    {label:'檔案名稱',width:'',prop:'s',col:'file_name',parameter:'N',hidden:'N',sort:7,defult:'',slot:false}, 
      {label:'未通過原因',width:'',prop:'w',col:'attestation_reason',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'認證發佈',width:'',prop:'x',col:'attestation_release',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'可上傳檔案數',width:'',prop:'x_file_cnt',col:'x_file_cnt',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:true,
    download_file:true,    
    add_data:false,
    course_select:true
}

//確認課程學習成果
const stu_attestation_confirm =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:'',slot:false},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:'',slot:false},
	    {label:'班級',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目代碼',width:'',prop:'e',col:'sub_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'分組',width:'',prop:'f',col:'src_dup',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'教師代碼',width:'',prop:'g',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'學生',width:'',prop:'h',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'序號',width:'',prop:'i',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'學分數',width:'',prop:'j',col:'credit',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
      {label:'修習方式',width:'',prop:'t',col:'scr_study',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
      {label:'送出認證',width:'',prop:'l',col:'attestation_send',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},
      {label:'認證日期',width:'',prop:'m',col:'attestation_date',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'認證狀態',width:'',prop:'n',col:'attestation_status',parameter:'N',hidden:'Y',sort:0,defult:'',slot:true},
      {label:'資料來源',width:'',prop:'o',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'備註',width:'',prop:'p',col:'content',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目名稱',width:'',prop:'q',col:'sub_name',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
	    {label:'教師名稱',width:'',prop:'r',col:'emp_name',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
	    {label:'檔案名稱',width:'',prop:'s',col:'file_name',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},            
      {label:'確認課程學習成果',width:'',prop:'u',col:'attestation_confirm',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'課程學習成果勾選狀態',width:'',prop:'v',col:'attestation_centraldb',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},       
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:false,
    download_file:true,    
    add_data:false,
    checkbox:false,
    confirm_batch:true
}

//確認課程學習成果
const stu_attestation_centraldb =
{
    header:
    [
      {label:'學校代碼',width:'',prop:'a',col:'sch_no',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'年度',width:'50',prop:'b',col:'year_id',parameter:'Y',hidden:'N',sort:1,defult:'',slot:false},
      {label:'學期',width:'50',prop:'c',col:'sms_id',parameter:'Y',hidden:'N',sort:2,defult:'',slot:false},
	    {label:'班級',width:'',prop:'d',col:'cls_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目代碼',width:'',prop:'e',col:'sub_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'分組',width:'',prop:'f',col:'src_dup',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'教師代碼',width:'',prop:'g',col:'emp_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'學生',width:'',prop:'h',col:'std_no',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'序號',width:'',prop:'i',col:'ser_id',parameter:'Y',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'學分數',width:'',prop:'j',col:'credit',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
      {label:'修習方式',width:'',prop:'t',col:'scr_study',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
      {label:'送出認證',width:'',prop:'l',col:'attestation_send',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'認證日期',width:'',prop:'m',col:'attestation_date',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'認證狀態',width:'',prop:'n',col:'attestation_status',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'資料來源',width:'',prop:'o',col:'is_sys',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
      {label:'備註',width:'',prop:'p',col:'content',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false},
	    {label:'科目名稱',width:'',prop:'q',col:'sub_name',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
	    {label:'教師名稱',width:'',prop:'r',col:'emp_name',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
	    {label:'檔案名稱',width:'',prop:'s',col:'file_name',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},            
      {label:'確認課程學習成果',width:'',prop:'u',col:'attestation_confirm',parameter:'N',hidden:'Y',sort:0,defult:'',slot:false}, 
      {label:'課程學習成果勾選狀態',width:'',prop:'v',col:'attestation_centraldb',parameter:'N',hidden:'N',sort:0,defult:'',slot:true}, 
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:false,
    download_file:true,    
    add_data:false,
    checkbox:false,
    centraldb_batch:true
}
//確認多元學習
const stu_diverse_check =
{
    leftheader:
    [
      {label:'幹部經歷紀錄',width:'',prop:'stu_cadre',col:'stu_cadre',parameter:'N',hidden:'N',sort:1,defult:'',slot:false},
      {label:'競賽參與紀錄',width:'',prop:'stu_competition',col:'stu_competition',parameter:'N',hidden:'N',sort:2,defult:'',slot:false},
      {label:'檢定證照紀錄',width:'',prop:'stu_license',col:'stu_license',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
	    {label:'檢視志工服務紀錄',width:'',prop:'stu_volunteer',col:'stu_volunteer',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
	    {label:'作品成果紀錄',width:'',prop:'stu_result',col:'stu_result',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
	    {label:'其他活動紀錄',width:'',prop:'stu_ohter',col:'stu_ohter',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
      {label:'彈性學習紀錄',width:'',prop:'stu_study_f',col:'stu_study_f',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},
	    {label:'職場學習紀錄',width:'',prop:'stu_workplace',col:'stu_workplace',parameter:'N',hidden:'N',sort:8,defult:'',slot:false},
      {label:'大學及技專校院先修課程紀錄',width:'',prop:'stu_college',col:'stu_college',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
      {label:'團體活動時間紀錄',width:'',prop:'stu_group',col:'stu_group',parameter:'N',hidden:'N',sort:10,defult:'',slot:false},
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:false,
    download_file:true,    
    add_data:false,
    checkbox:false,
    centraldb_batch:true,
}
//學習歷程資料匯出
const stu_turn_export =
{
    header:
    [
      {label:'班級',width:'',prop:'cls_abr',col:'cls_abr',parameter:'N',hidden:'N',sort:1,defult:'',slot:false},
      {label:'學號',width:'',prop:'std_no',col:'std_no',parameter:'N',hidden:'N',sort:2,defult:'',slot:false},
      {label:'姓名',width:'',prop:'std_name',col:'std_name',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
    ],
    sub_header:
    [
      {label:'班級',width:'',prop:'cls_abr',col:'cls_abr',parameter:'N',hidden:'N',sort:1,defult:''},
      {label:'學號',width:'',prop:'std_no',col:'std_no',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'姓名',width:'',prop:'std_name',col:'std_name',parameter:'N',hidden:'N',sort:3,defult:''},
    ],
    file_header:
    [
      {label:'檔案名稱',width:'',prop:'file_name',col:'file_name',parameter:'N',hidden:'N',sort:1,defult:''},
      {label:'壓縮密碼',width:'',prop:'zipcode',col:'zipcode',parameter:'Y',hidden:'N',sort:2,defult:''},
      {label:'MD5密碼',width:'',prop:'file_md5',col:'file_md5',parameter:'N',hidden:'N',sort:3,defult:''},   
      {label:'檔案產生日期',width:'',prop:'upd_dt',col:'upd_dt',parameter:'N',hidden:'N',sort:3,defult:''},  
      {label:'檔案產生學生資訊',width:'',prop:'stulist',col:'stulist',parameter:'N',hidden:'N',sort:3,defult:''}, 
    ],
    delete_rule:{},
    save_rule:{},
    query_data:false,
    delete_data:false,
    upload_file:false,
    download_file:false,    
    add_data:false,
    checkbox:false,
    centraldb_batch:false,
}

async function getOpenOpYN(vue,apiurl,parameter,token){
  let _self = this
  let open_yn ='N'
  let obj={open_yn:'',startdate:'',enddate:''}
  let start = ''
  let end = ''

  try {
      const { data, statusText } = await adminAPI.get_OpOpenYN.Get(parameter) 

      if (statusText !== "OK") {
        throw new Error(statusText);
      }
      if (data.dataset.open_yn == 'Y') {       
          start = data.dataset.startdate
          end = data.dataset.enddate   
          obj.open_yn ='Y'
          obj.startdate = start.substr(0,4)+'/'+start.substr(4,2)+'/'+start.substr(6,2)+' '+start.substr(8,2)+':'+start.substr(10,2)+':'+start.substr(12,2)
          obj.enddate = end.substr(0,4)+'/'+end.substr(4,2)+'/'+end.substr(6,2)+' '+end.substr(8,2)+':'+end.substr(10,2)+':'+end.substr(12,2)
      } else {      
        start = data.dataset.startdate
        end = data.dataset.enddate   
        if(!start && typeof(start) !== 'undefined' && start != 0)
        {
          obj.open_yn ='N'
          obj.startdate = ''
          obj.enddate = ''
        }
        else{       
          obj.open_yn ='N'
          obj.startdate = start.substr(0,4)+'/'+start.substr(4,2)+'/'+start.substr(6,2)+' '+start.substr(8,2)+':'+start.substr(10,2)+':'+start.substr(12,2)
          obj.enddate = end.substr(0,4)+'/'+end.substr(4,2)+'/'+end.substr(6,2)+' '+end.substr(8,2)+':'+end.substr(10,2)+':'+end.substr(12,2)
        }
      }    
  } catch (error) {
    
  }

  return obj
}

export
{
  stu_cadre,
  stu_study_f,
  stu_group,
  stu_college,
  stu_workplace,
  stu_other,
  stu_result,
  stu_volunteer,
  stu_license,
  stu_competition,
  consult_query,
  stu_attestation,
  stu_attestation_confirm,
  stu_attestation_centraldb,
  stu_diverse_check,
  stu_turn_export,
  stu_attestation_course,
  getOpenOpYN
}
