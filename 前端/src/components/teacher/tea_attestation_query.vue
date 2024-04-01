<template>
  <div>
    <el-container>
      <el-header>
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
              <el-form-item label="班級:">
                <el-select v-model="queryform.cls_id" placeholder="">
                  <el-option v-for="item in clslist" :key="item.cls_id" :label=" item.cls_abr" :value="item.cls_id" >
                  </el-option>        
                </el-select>                      
              </el-form-item>
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
      </el-header>
      <br/>
      <br/>
      <el-main>
        <el-table :data="tableData" stripe style="width: 100%" :row-style="rowState">
          <!--
            <el-table-column type="index" width="50"></el-table-column>
          -->
          <el-table-column v-for="item in render_header"
              :key="item.prop"
              :prop="item.prop"
              :label="item.label"
              :width="item.width"
              align="center"
          >  
          </el-table-column>
        </el-table>
        <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
          v-on:current-change="current_change" v-on:size-change="size_change">
        </el-pagination>          
      </el-main>
    </el-container>    
  </div>
</template>

<script type="module">
var apiurl = ''
import datalistyear from '@/components/pub/DataList_year.vue'  
import datalistsms from '@/components/pub/DataList_sms.vue'  
import datalistcls from '@/components/pub/DataList_stucls.vue'  
import tea_attestation_query from '@/components/teacher/tea_attestation_query.vue'
import * as data_structure from '@/js/tea_grid_structure.js'

export default {
props: {
  userStatic: {
    type: Object,
  },
  api_interface: {
    type: Object,
  },
  activePage:{
    type:String
  } 
},
data() {
  return {
      tableData:[],
      data_structure: {},
      queryform:{
                year_id:"",
                sms_id:"",
                cls_id:'',  
                std_no:'',
                std_name:'',
                emp_id:"",
                sRowNun:1,
                eRowNun:10,
                token:this.$token,
      },
      yearlist:[],
      smslist:[],
      clslist:[],
      clsParms:{year_id:'',sms_id:'',emp_id:'',token:this.$token},
      total: 0,
      currentPage: 1,
      pageSize: 10,
  }
},
methods: {
yearChange:function(val){
    this.clsParms.year_id = val     
    this.getclslist(this.clsParms)
},
smsChange:function(val){
    this.clsParms.sms_id = val
    this.getclslist(this.clsParms)
},
rowState(row, rowindex) {
    return {
      backgroundColor: '#f4f4f5',
    }
},   
current_change(val) {
      var _self = this;
      var start = ''
      var end = ''

      start = ((val - 1) * _self.pageSize) + 1;
      end = val * _self.pageSize
      _self.currentPage = val
      _self.queryform.sRowNun = start
      _self.queryform.eRowNun = end
      this.get_data()
    },
  size_change(val) {
      var _self = this;
      var start = ''
      var end = ''
      start = ((_self.currentPage - 1) * val) + 1;
      end = _self.currentPage * val
      _self.pageSize = val
      _self.queryform.sRowNun = start
      _self.queryform.eRowNun = end
      this.get_data()
  },  
  get_data:function(apiurl){
      let _self = this
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          if(_self.queryform.std_name != '' || _self.queryform.std_no != ''){
            _self.queryform.sRowNun = 1
            _self.queryform.eRowNun = 10
            _self.currentPage = 1
            _self.pageSize = 10
          }
          _self.$http({
            url: _self.api_interface.get_attestationResult,
            method: 'get',
            params: _self.queryform,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
              if (res.data.status == 'Y') {
                _self.tableData = res.data.dataset
                _self.total = res.data.dataset[0].x_total
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
  query:function(){     
    this.get_data()
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
                                          reject('N1')
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
                        reject('N2')
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
                        reject('N3')
                      })
              .finally()        
    }) 
  },
  asyncRun:async function(){
    await this.getyearlist()
    await this.getsmslist()
    await this.getclslist(this.clsParms)
  },  
},
components: {
  tea_attestation_query,
  datalistyear,
  datalistsms,
  datalistcls
},
beforeDestroy(){

},
mounted() {
  this.asyncRun()  
},
beforeMount() {
  switch (this.userStatic.data_structure) {
    case 'tea_attestation_query'://授課教師認證課程學習成果
      this.data_structure = data_structure.tea_attestation_query
      break;
    default:
  }
},
computed: {
  render_header() {      
    let headers = this.data_structure.header.filter((a) => { return a.hidden === 'N' })
      .sort(function (a, b) {
        if (a.sort > b.sort) {
          return 1
        } else {
          return -1
        }
      }
      )
    return headers
  }
},
watch:{

},
created() {

}
}

</script>

<style>

</style>