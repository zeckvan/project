<template>
  <div>
  <div align="left">
        <el-form :inline="true" :model="queryform">
            <el-form-item label="學年：">      
              <el-select v-model="queryform.year_id" placeholder="學年" @change="yearChange">
                  <el-option v-for="item in yearlist" :key="item.year_id" :label="item.year_id" :value="item.year_id" >
                  </el-option>        
              </el-select>         
            </el-form-item>
            <el-form-item label="學期:">
              <el-select v-model="queryform.sms_id" placeholder="學期" @change="smsChange">
                <el-option v-for="item in smslist" :key="item.sms_id" :label="item.sms_abr" :value="item.sms_id" >
                </el-option>        
              </el-select>   
            </el-form-item>
            <!--
            <el-form-item label="班級:">
              <el-select v-model="queryform.cls_id" placeholder="">
                <el-option v-for="item in clslist" :key="item.cls_id" :label=" item.cls_abr" :value="item.cls_id" >
                </el-option>        
              </el-select>                      
            </el-form-item>
            -->
            <el-form-item label="學號:">
              <el-input v-model="queryform.std_no"></el-input>
            </el-form-item>
            <el-form-item label="學生姓名:">
              <el-input v-model="queryform.std_name"></el-input> 
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="query">查詢</el-button>
            </el-form-item>
        </el-form>
    </div>
    <el-tabs v-model="activeName" @tab-click="handleClick">     
        <el-tab-pane label="學習成果概況統計" name="first">
          <LearningResult 
            :userStatic="userStatic" 
            :tableDataPass="tableData"
            v-on:get-std="getstd"
            v-on:dbclick-row="dbclickrow"
            :totalPass="total"
            :api_interface="api_interface"
            :queryform="queryform"
          />
        </el-tab-pane>
        <el-tab-pane :label="page2Lable" name="second">
          <TeaTutorDetail
            :queryform="queryform"
            :cleanlist="cleanlist"
            :List="List"
          />
        </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
  var apiurl = ''
  import LearningResult from '@/components/admin/MultipleLearning.vue'
  import TeaTutorDetail from '@/components/teacher/tea_tutor_detail.vue' 
  import * as adminAPI from  '@/apis/adminApi.js' 
  import * as teaAPI from  '@/apis/teaApi.js' 
  export default {
    name: 'AdmintTutorView',
    props: {
        userStatic: {
          type: Object,
        },
        api_interface: {
          type: Object,
        },
    },
    data: function () {
                return {
                    activeName: 'first',
                    activePage:'0',
                    tableData:[],
                    data_structure: {},
                    queryform:{
                              year_id:"",
                              sms_id:"",
                              cls_id:'',  
                              std_no:'',
                              std_name:'',
                              emp_id:"",
                              consult_emp:"",
                              kind:this.userStatic.interface,
                              sRowNun:1,
                              eRowNun:10,token:this.$token,arg_std:''},
                      yearlist:[],
                      smslist:[],
                      clslist:[],
                      cleanlist:[1,2,3],
                      clsParms:{year_id:'',sms_id:'',emp_id:'10615'},
                      total: 0,
                      currentPage: 1,
                      pageSize: 10,
                      page2Lable:'多元表現概況明細',
                      List:[
                            {diverseid:'多元學習-幹部經歷紀錄',width:'',prop:'',col:'stucadre',parameter:'N',hidden:'N',sort:1,defult:'',slot:false},
                            {diverseid:'多元學習-競賽參與紀錄',width:'',prop:'',col:'stucompetition',parameter:'N',hidden:'N',sort:2,defult:'',slot:false},
                            {diverseid:'多元學習-檢定證照紀錄',width:'',prop:'',col:'stulicense',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
                            {diverseid:'多元學習-檢視志工服務紀錄',width:'',prop:'',col:'stuvolunteer',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
                            {diverseid:'多元學習-作品成果紀錄',width:'',prop:'',col:'sturesult',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
                            {diverseid:'多元學習-其他活動紀錄',width:'',prop:'',col:'stuother',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
                            {diverseid:'多元學習-彈性學習紀錄',width:'',prop:'',col:'stustudyf',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},
                            {diverseid:'多元學習-職場學習紀錄',width:'',prop:'',col:'stuworkplace',parameter:'N',hidden:'N',sort:8,defult:'',slot:false},
                            {diverseid:'多元學習-大學及技專校院先修課程紀錄',width:'',prop:'',col:'stucollege',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
                            {diverseid:'多元學習-團體活動時間紀錄',width:'',prop:'',col:'stugroup',parameter:'N',hidden:'N',sort:10,defult:'',slot:false},   
                        ],                        
                }
            },
    components: {
      LearningResult,
      TeaTutorDetail
    },
    methods: {
      getstd:function(val){
        this.queryform.arg_std = val
        this.page2Lable = `多元表現概況明細(學號：${val})`
      },
      yearChange:function(val){
          this.clsParms.year_id = val     
      },
      smsChange:function(val){
          this.clsParms.sms_id = val
      },
      query:function(){     
        this.page2Lable = `多元表現概況明細(學號：)`
        this.get_data()
      },   
      get_data:async function(){
        let _self = this

        const { data, statusText } = await teaAPI.TeaAttestation.MultipleLearning.Get(_self.queryform) 

        if (statusText !== "OK") {
          throw new Error(statusText);
        }

        if (data.status == 'Y') {     
            _self.tableData = data.dataset   
            _self.total = data.dataset[0].x_total
            _self.queryform.arg_std = ''
            _self.page2Lable = `多元表現概況明細(學號：)`
          } else {
            _self.tableData = []
            _self.$message.error('查無資料，請確認')
          }
      }, 
      handleClick:function(tab, event){
          this.activePage = tab.index
      }, 
      dbclickrow:function(){
        this.cleanlist = []
        this.activeName = 'second'
      },
      getyearlist:async function(){
          let _self = this

          const { data, statusText } = await adminAPI.s90yearinfo.Get() 

          if (statusText !== "OK") {
            throw new Error(statusText);
          }

          if(data.status == 'Y'){
            _self.yearlist = data.dataset
            _self.queryform.year_id = data.dataset[0].year_id.toString()
            _self.clsParms.year_id = data.dataset[0].year_id.toString()
          }else{
            queryform.year_id = ""
            yearlist = []
          } 
      },
      getsmslist:async function(){
          let _self = this

          const { data, statusText } = await adminAPI.s90smsinfo.Get() 

          if (statusText !== "OK") {
            throw new Error(statusText);
          }

          if(data.status == 'Y'){
            _self.smslist = data.dataset
            _self.queryform.sms_id = data.dataset[0].sms_id.toString()
            _self.clsParms.sms_id = data.dataset[0].sms_id.toString()
          }else{
            _self.queryform.sms_id = ""
            _self.smslist = []
          }  
      },
      getclslist:function(arg){
          let _self = this    
          const apiurl = `${_self.$apiroot}/TeaAttestation/clslist`      
          return new Promise(function(resolve, reject){
            _self.$http({
                    url:apiurl,
                    method:'get',
                    params:arg,
                    headers:{'SkyGet':_self.$token}
                    })
                    .then((res)=>{                   
                          _self.clslist = [],
                          _self.queryform.cls = '' 
                          if(res.data.status == 'Y'){
                            _self.clslist = res.data.dataset                  
                            _self.queryform.cls_id = res.data.dataset[0].cls_id.toString()
                          }else{
                            _self.queryform.cls_id = ""
                            _self.clslist = []                      
                          }     
                          resolve('Y')                               
                      })         
                    .catch((error)=>{
                              _self.$message.error('呼叫後端【S04_stucls】發生錯誤,'+error)
                              reject('N')
                            })
                    .finally()        
          }) 
      },
      asyncRun:async function(){      
        const promises = [
            this.getyearlist(),
            this.getsmslist()
        ];

        await Promise.allSettled(promises)
        /*
        await Promise.allSettled(promises).then(values => {
         console.log(values)
        });           
         */
           
        //await this.getyearlist()
        //await this.getsmslist()
        //await this.getclslist(this.clsParms)
      },                   
    },
    mounted: function () {

    },
    beforeDestroy(){

    },
    mounted() {
      this.asyncRun()  
    },    
  }
</script>
