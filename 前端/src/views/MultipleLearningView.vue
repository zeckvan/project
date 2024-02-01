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
                              eRowNun:10,token:this.$token},
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
        this.queryform.std_no = val
        this.page2Lable = `學習成果及多元表現概況明細(學號：${val})`
      },
      yearChange:function(val){
          this.clsParms.year_id = val     
          //this.getclslist(this.clsParms)
      },
      smsChange:function(val){
          this.clsParms.sms_id = val
          //this.getclslist(this.clsParms)
      },
      query:function(){     
        this.get_data()
      },   
      get_data:function(){
        let _self = this
        const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });
      apiurl = _self.api_interface.get_data

      _self.$http({
        url: apiurl,
        method: 'get',
        params: _self.queryform,
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {  
          if (res.data.status == 'Y') {     
            _self.tableData = res.data.dataset   
            _self.total = res.data.dataset[0].x_total
            console.log(_self.total )
          } else {
            _self.tableData = []
            _self.$message.error('查無資料，請確認')
          }
        })
        .catch((error) => {
            _self.tableData = []
            _self.$message({
              message: '系統發生錯誤'+error,
              type: 'error',
              duration:0,
              showClose: true,
            })
          })
        .finally(() => loading.close())        
      }, 
      handleClick:function(tab, event){
          this.activePage = tab.index
      }, 
      dbclickrow:function(){
        this.cleanlist = []
        this.activeName = 'second'
      },
      getyearlist:function(){
          let _self = this
          const apiurl = `${_self.$apiroot}/s90yearinfo`   
          return new Promise(function(resolve, reject){
                              _self.$http({
                                      url:apiurl,
                                      method:'get',
                                      headers:{'SkyGet':_self.$token}
                                      })
                                      .then((res)=>{        
                                            if(res.data.status == 'Y'){
                                              _self.yearlist = res.data.dataset
                                              _self.queryform.year_id = res.data.dataset[0].year_id.toString()
                                              _self.clsParms.year_id = res.data.dataset[0].year_id.toString()
                                            }else{
                                              _self.queryform.year_id = ""
                                              _self.yearlist = []
                                            }  
                                            resolve('Y')
                                        })         
                                      .catch((error)=>{
                                                _self.$message.error('呼叫後端【s90yearinfo】發生錯誤,'+error)
                                                reject('N')
                                              })
                                      .finally()                             
                            });      
        },
        getsmslist:function(){
          let _self = this    
          const apiurl = `${_self.$apiroot}/s90smsinfo`      
          return new Promise(function(resolve, reject){
                    _self.$http({
                    url:apiurl,
                    method:'get',
                    headers:{'SkyGet':_self.$token}
                    })
                    .then((res)=>{
                        if(res.data.status == 'Y'){
                          _self.smslist = res.data.dataset
                          _self.queryform.sms_id = res.data.dataset[0].sms_id.toString()
                          _self.clsParms.sms_id = res.data.dataset[0].sms_id.toString()
                        }else{
                          _self.queryform.sms_id = ""
                          _self.smslist = []
                        }  
                        resolve('Y')                                                     
                      })         
                    .catch((error)=>{
                              _self.$message.error('呼叫後端【s90smsinfo】發生錯誤,'+error)
                              reject('N')
                            })
                    .finally()  
          })      
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
        await this.getyearlist()
        await this.getsmslist()
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
