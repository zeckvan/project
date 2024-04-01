<template>
  <div>      
    <el-container>
      <el-aside width="250px">
        <el-table :data="List" 
                  height=73vh 
                  stripe 
                  :row-style="rowState" 
                  v-on:row-click="rowclick" 
                  style="border: 3px solid #eee;">
              <el-table-column type="index" width="50"></el-table-column>
              <el-table-column 
              prop="diverseid"
              label="項目名稱">
              </el-table-column>
          </el-table>         
      </el-aside>
      <el-main>
        <el-table :data="tableData" height=73vh stripe :row-style="rowState2" v-on:header-click="headerclick" style="border: 3px solid #eee;">
                  <el-table-column type="index" width="50"></el-table-column>     
                  <el-table-column  v-for="item in render_header"
                  :key="item.prop"
                  :prop="item.prop"
                  :label="item.label"
                  :width="item.width"   
                  align="center">
                  <template slot-scope="scope" >
                    <div v-if="item.slot">
                      <div v-if="item.col == 'attestation_send'">
                        <!--
                        <div v-if="scope.row.l == '' || scope.row.l == null || (scope.row.n == 'F' && scope.row.x == 'Y')">
                          <el-button type="primary" size="small " v-on:click="send(scope)">送出認證</el-button>        
                        </div>
                        <div v-else>
                          {{scope.row[item.prop]}}
                        </div>     
                        -->
                        {{scope.row[item.prop]}}
                      </div>
                      <div v-else-if="item.col == 'attestation_centraldb'">
                        <div align="center">
                          <el-checkbox v-model="scope.row.v"></el-checkbox> 
                        </div>
                      </div>            
                      <div v-else>
                        <div v-if="scope.row.n == 'N' && (scope.row.x == '' || scope.row.x == null) ">
                          <el-tag type="success" effect="dark">認證中</el-tag>
                        </div>
                        <div v-else-if="scope.row.n == 'Y' && scope.row.x == 'Y'">
                          <el-tag type="primary" effect="dark">認證成功</el-tag>
                        </div>   
                        <div v-else-if="scope.row.n == 'F' && scope.row.x == 'Y'">                
                          <el-popover
                            placement="top-start"
                            title="認證失敗原因："
                            width="200"
                            trigger="hover"
                            :content="scope.row.w">
                            <el-tag type="danger" effect="dark" slot="reference">認證失敗</el-tag>
                          </el-popover>                
                        </div>                  
                      </div>
                    </div>
                    <div v-else>
                      {{scope.row[item.prop]}}
                    </div>
                  </template>                    
                  </el-table-column>
                  <el-table-column align="center" label="檢視資料" width="" v-if="data_structure.query_data">
                      <template slot-scope="scope">
                      <i class="el-icon-search"  @click="edit_data(scope)"></i>
                      </template>
                  </el-table-column>
                  <el-table-column align="center" label="已勾選確認檔案數" width="" v-if="this.userStatic.data_structure == 'stuattestationconfirm'">
                      <template slot-scope="scope">
                        <el-badge :value="scope.row.x_file_center_cnt" class="item" type="primary"></el-badge>
                      </template>
                  </el-table-column>                      
                  <el-table-column align="center"  label="查看檔案" width="" v-if="data_structure.download_file">
                      <template slot-scope="scope">
                          <el-badge :value="scope.row.x_cnt" class="item" type="info"  @click.native="file_data(scope)" style="cursor:pointer"></el-badge>
                          <!--
                              <i class="el-icon-s-order" style="cursor:pointer" @click="file_data(scope)"></i>
                          -->
                      </template>
                  </el-table-column>
         </el-table>

        <component :is='content' :dialog_show.sync="dialog_show" :edit_model="edit_model" :parameter="parameter"
                    :data_structure="data_structure" :userStatic="userStatic" :api_interface="api_interface" v-on:get-show="getshow"
                    v-if="isShow" :formSaveCheck="formSaveCheck">
        </component>

        <v-stufilelist :dialog_show.sync="dialog_show2" :filelist="filelist" :userStatic="userStatic" :parameter="parameter"
                        :api_interface="api_interface" v-on:get-show="getshow2" v-if="isShow2">
        </v-stufilelist>

        <v-fileupload :dialog_show.sync="dialog_show3" :filelist="filelist" :userStatic="userStatic"
                        :api_interface="api_interface" :parameter="parameter" :data_structure="data_structure" :rowdata="rowdata"
                        v-on:get-show="getshow3" v-if="isShow3">
        </v-fileupload>             
      </el-main>
    </el-container>                              
  </div>
</template>   

<script type="module">
var apiurl = ''
import stufilelist from '@/components/student/stu_pub_filelist.vue'
import stucdreform from '@/components/student/stu_cadre_form.vue'
import stustudyform from '@/components/student/stu_study_form.vue'
import stugroupform from '@/components/student/stu_group_form.vue'
import stucollegeform from '@/components/student/stu_college_form.vue'
import stuworkplaceform from '@/components/student/stu_workplace_form.vue'
import stuotherform from '@/components/student/stu_other_form.vue'
import sturesultform from '@/components/student/stu_result_form.vue'
import stuvolunteerform from '@/components/student/stu_volunteer_form.vue'
import stulicenseform from '@/components/student/stu_license_form.vue'
import stucompetitionform from '@/components/student/stu_competition_form.vue'
import stuconsultform from '@/components/student/stu_consult_form.vue'
import PubQuery from '@/components/pub/pub_query.vue'
import fileupload from '@/components/pub/pub_upload.vue'
import * as data_structure from '@/js/stu_grid_structure.js'
import * as stuAPI from  '@/apis/stuApi.js' 
export default {
props: {
  queryform:{
    type:Object
  },
  cleanlist:{
    type:Array
  },
  List:{
    type:Array
  }
},
data() {
  return {
    opno:'',
    formSaveCheck:'studiversecheck',
    userStatic:{},
    api_interface:{},
    content:'',
    data_structure: {},
    tableData: [],
    isShow: false,
    isShow2: false,
    isShow3: false,
    dialog_show: true,
    dialog_show2: true,
    dialog_show3: true,
    total: 0,
    currentPage: 1,
    pageSize: 10,
    year: '',
    sms: '',
    edit_model: '',
    parameter: {},
    filelist: [],
    rowdata: {},
    check_list:[],
    leftList:[
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
                  {diverseid:'學習成果-上傳課程學習成果',width:'',prop:'',col:'stuattestation',parameter:'N',hidden:'N',sort:11,defult:'',slot:false},   
                  {diverseid:'學習成果-學習成果認證',width:'',prop:'',col:'stuattestationconfirm',parameter:'N',hidden:'N',sort:12,defult:'',slot:false},   
                  {diverseid:'學習成果-勾選課程學習成果',width:'',prop:'',col:'stuattestationcentraldb',parameter:'N',hidden:'N',sort:13,defult:'',slot:false},   
              ],    
    yearlist:[],
    getIndex:'',
    flag:'N',
    item:''
  }
},
methods: {
  query:function(){
      
  },
  yearChange:function(){

  },
  rowclick:function(row, column, event){
      this.getIndex = row.sort
      let _self = this
      _self.opno = row.col
      _self.item = row.col
      switch (row.col) {  
          case 'studiversecheck'://勾選多元學習
              this.data_structure = data_structure.stu_diverse_check
              break;   
          case 'stucadre'://幹部經歷紀錄
              this.data_structure = data_structure.stu_cadre
              break;
          case 'stustudyf'://彈性學習時間紀錄
              this.data_structure = data_structure.stu_study_f
              break;
          case 'stugroup'://團體活動時間紀錄
              this.data_structure = data_structure.stu_group
              break;
          case 'stucollege'://大學及技專校院先修課程紀錄
              this.data_structure = data_structure.stu_college
              break;
          case 'stuworkplace'://職場學習紀錄
              this.data_structure = data_structure.stu_workplace
              break;
          case 'stuother'://其他多元表現紀錄
              this.data_structure = data_structure.stu_other
              break;
          case 'sturesult'://作品成果紀錄
              this.data_structure = data_structure.stu_result
              break;
          case 'stuvolunteer'://服務學習紀錄
              this.data_structure = data_structure.stu_volunteer
              break;
          case 'stulicense'://檢定證照紀錄
              this.data_structure = data_structure.stu_license
              break;
          case 'stucompetition'://競賽參與紀錄
              this.data_structure = data_structure.stu_competition
              break;
          case 'stuconsult_query'://學生諮詢課程查詢
              this.data_structure = data_structure.consult_query
              break;
          case 'stuattestation'://學生諮詢課程查詢
              this.data_structure = data_structure.stu_attestation
              break;
          case 'stuattestationconfirm'://確認課程學習成果
              this.data_structure = data_structure.stu_attestation_confirm
              this.flag = 'Y'
              break;
          case 'stuattestationcentraldb'://確認課程學習成果
              this.data_structure = data_structure.stu_attestation_centraldb
              break;           
          default:
          }   
          this.$router.options.routes.find(function(item,index,array){
              if(item.name == (row.col)){
                _self.api_interface = item.props.api_interface
                _self.userStatic = item.props.userStatic
                _self.userStatic.file_delete = false
                _self.userStatic.checkbox = true
                _self.content = item.props.userStatic.form_component
              }
          })
          _self.tableData = []
          _self.getcondition()
  },
  centraldb_batch:function(){
        var i = 0;
        let _self = this
        let formdata = new FormData();

        _self.check_list.length = 0;

        this.tableData.forEach((element, index) => {
          _self.check_list[i++] = element
        });
/*
        if(_self.check_list.length == 0){
          _self.$message.error('未勾選課程學習成果，請確認!!');
          return false;
        }     
*/

        _self.check_list.forEach(function(value, index, array){
          if(_self.opno == 'stucadre'){
            formdata.append('complex_array[]',value.a +'_'+value.c+'_'+value.d+'_'+value.b+'_'+value.e+'_'+value.h+'@'+value.x_status)
          }else{
            formdata.append('complex_array[]',value.a +'_'+value.b+'_'+value.c+'_'+value.d+'_'+value.e+'@'+value.x_status)
          }

        });


      const loading = _self.$loading({
            lock: true,
            text: '資料處理中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
            });

      const apiurl = _self.api_interface.save_centraldb
      _self.$http({
          url:apiurl,
          method:"put",
          data:formdata,
          headers:{'SkyGet':_self.$token}
          })
          .then((res)=>{
                          if (res.data.status == 'Y'){
                              _self.$message.success('存檔成功!!')
                              var obj = {
                                          year:_self.year,
                                          sms: _self.sms
                                        }
                                        
                                    _self.getcondition(obj)
                          }else{
                              _self.$message.error(res.data.message)
                          }
                  })
          .catch((error) => {
              _self.$message({
              message: '存檔失敗:'+error,
              type: 'error',
              duration:0,
              showClose: true,
              })
          })
          .finally(()=>loading.close())      
        
  },
  confirm_batch:function(){
        var i = 0;
        let _self = this
        let formdata = new FormData();

        _self.check_list.length = 0;

        this.tableData.forEach((element, index) => {
          if(element.x_status == true){
            _self.check_list[i++] = element
          }
        });

        if(_self.check_list.length == 0){
          _self.$message.error('未勾選學習成果認證，請確認!!');
          return false;
        }     

        _self.check_list.forEach(function(value, index, array){
          formdata.append('complex_key[]',value.a +'_'+value.b+'_'+value.c+'_'+value.d+'_'+value.e+'_'+value.f+'_'+value.g+'_'+value.h)
        });

      _self.$confirm(`確定送出學習成果認證?`, 'Warning', {
        confirmButtonText: '確定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        const loading = _self.$loading({
              lock: true,
              text: '資料處理中，請稍後。',
              spinner: 'el-icon-loading',
              background: 'rgba(0, 0, 0, 0.7)'
              });

      const apiurl = _self.api_interface.save_form
      _self.$http({
          url:apiurl,
          method:"put",
          data:formdata,
          headers:{'SkyGet':_self.$token}
          })
          .then((res)=>{
                          if (res.data.status == 'Y'){
                              _self.$message.success('送出成功!!')
                              var obj = {
                                          year:_self.year,
                                          sms: _self.sms
                                        }
                                        
                                    _self.getcondition(obj)
                          }else{
                              _self.$message.error(res.data.message)
                          }
                  })
          .catch((error) => {
              _self.$message({
              message: '送出失敗:'+error,
              type: 'error',
              duration:0,
              showClose: true,
              })
          })
          .finally(()=>loading.close())      
      }).catch(() => {
      })          
  },
  cellmouseenter(row, column, cell, event){
           
  },     
  headerclick(column, event){
    if(column.label == '全選' || column.label == '全不選'){
      if(column.label == '全選'){
      column.label = '全不選'
      this.tableData.forEach((element, index) => {
        if(element.x_status == false || element.x_status == ''){
          element.x_status = true
        }
      });
      }else{
        column.label = '全選'
        this.tableData.forEach((element, index) => {
          if(element.x_status == true || element.x_status == ''){
            element.x_status = false
          }
        });
      }
    }
  },    
  send:function(val){
    let _self = this
    let formdata = new FormData();

    if(val.row.x_cnt == 0){
      _self.$message.warning ("請先上傳檔案後，再執行送出認證。")
      return false;
    }

    this.data_structure.header.forEach(function(value, index, array){
            formdata.append(value.col,val.row[value.prop]);
        });

     _self.$confirm(`確定送出?一但送出後該項目無法修正!!`, 'Warning', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(() => {
      const loading = _self.$loading({
            lock: true,
            text: '資料處理中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
            });

    const apiurl = _self.api_interface.save_form
    _self.$http({
        url:apiurl,
        method:"put",
        data:formdata,
        headers:{'SkyGet':_self.$token}
        })
        .then((res)=>{
                        if (res.data.status == 'Y'){
                            _self.$message.success('送出成功!!')
                            var obj = {
                                        year:_self.year,
                                        sms: _self.sms
                                      }
                                      
                                   _self.getcondition(obj)
                        }else{
                            _self.$message.error(res.data.message)
                        }
                })
        .catch((error) => {
            _self.$message({
            message: '送出失敗:'+error,
            type: 'error',
            duration:0,
            showClose: true,
            })
        })
        .finally(()=>loading.close())      
    }).catch(() => {
    })
  },
  rowState(row, rowindex) {
    if(this.getIndex == row.row.sort){
        return {
        backgroundColor: '#f4f4f5',
        cursor:'pointer',
        color:"blue",
        "font-weight": "bold"
      }
    }else{
      return {
        backgroundColor: '#f4f4f5',
        cursor:'pointer',
        color:"#606266"
      }    
    }      
  },
  rowState2(row, rowindex) {
    /*
    return {
      backgroundColor: '#f4f4f5'
    }
    */
  },    
  add: function () {
    this.edit_model = 'add'
    this.isShow = true
  },
  edit_data: function (val) {    
    let _self = this
    this.data_structure.header.forEach(function (value, index, array) {
      if (value.parameter == "Y") {
        _self.parameter[value.col] = val.row[value.prop]
      }
    });
    this.edit_model = 'edit'
    this.isShow = true
  },
  file_data: function (val) {
    let _self = this
    let complex_key = ''
    apiurl = _self.api_interface.file_data
    const loading = _self.$loading({
      lock: true,
      text: '資料讀取中，請稍後。',
      spinner: 'el-icon-loading',
      background: 'rgba(0, 0, 0, 0.7)'
    });

    _self.data_structure.header.forEach(function (value, index, array) {
      if (value.parameter == "Y") {
        _self.parameter[value.col] = val.row[value.prop]
      }
    });

    _self.parameter["class_name"] = _self.userStatic.interface
    _self.userStatic.file_delete = false
    _self.parameter["deleteFile"] = 'N'

    if(_self.userStatic.interface == 'StuAttestation'){
          complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}_${val.row.f}_${val.row.g}_${val.row.h}_0`
      }else if(_self.userStatic.interface == 'StudCadre'){
          complex_key = `${val.row.a}_${val.row.c}_${val.row.d}_${val.row.b}_${val.row.k}`                    
      }
      else{
          complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}`
      }

     _self.parameter["complex_key"] = complex_key
    /*
     if(_self.userStatic.data_structure == 'stuattestation' || _self.userStatic.data_structure == 'stuattestationcentraldb')
     {
      _self.parameter["flag"] =  'N'
     }    
     else
     {
      _self.parameter["flag"] =  'Y'
     }  
     */   
     _self.parameter["flag"] = _self.userStatic.data_structure
    _self.$http({
      url: apiurl,
      method: 'get',
      params: _self.parameter,
      headers:{'SkyGet':_self.$token}
    })
    .then((res) => {
      if (res.data.status == 'Y') {
        _self.filelist = res.data.dataset
        _self.filelist.forEach(function(item,index,array){
                  if(item.attestation_file_yn == 'Y'){
                    item.attestation_file_yn = true
                  }else{
                    item.attestation_file_yn = false
                  }                      
              })              
      } else {
        _self.filelist = []
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

    this.isShow2 = true
  },
  file_upload: function (val) {
    let _self = this
    if(_self.userStatic.interface == 'StuAttestation'){
      if(val.row.n == 'N' || val.row.n == 'Y'){
        _self.$message.warning ("該項目狀態為「認證中」或「認證成功」，無法執行檔案上傳功能。")
        return false;
      }
    }
    apiurl = _self.api_interface.file_upload
    _self.rowdata = val
    _self.data_structure.header.forEach(function (value, index, array) {
      if (value.parameter == "Y") {
        _self.parameter[value.col] = val.row[value.prop]
      }
    });
    this.isShow3 = true
  },
  del_data: function (val) {
    let _self = this
    if (this.data_structure.delete_rule.rule_flag === "Y") {
      if (!this.data_structure.delete_rule.rule_check(val, _self)) {
        return false
      }
    }
    _self.$confirm(`確定刪除?`, 'Warning', {
      confirmButtonText: '確定',
      cancelButtonText: '取消',
      type: 'warning'
    }).then(() => {
      const loading = _self.$loading({
      lock: true,
      text: '資料讀取中，請稍後。',
      spinner: 'el-icon-loading',
      background: 'rgba(0, 0, 0, 0.7)'
      });

      let formdata = new FormData();

      _self.data_structure.header.forEach(function (value, index, array) {
        if (value.parameter == "Y") {
          _self.parameter[value.col] = val.row[value.prop]
          formdata.append(value.col, val.row[value.prop]);
        }
      });

      formdata.append("class_name",_self.userStatic.interface)
      apiurl = _self.api_interface.del_data
      _self.$http({
        headers: { 'Content-Type': 'application/json;charset=utf-8','SkyGet':_self.$token },
        url: apiurl,
        method: 'delete',
        data: formdata
      })
        .then((res) => {
          if (res.data.status == 'Y') {
            _self.$message.success('刪除成功!!')
            _self.tableData.splice(val.$index, 1);
          } else {
            _self.$message.error('刪除失敗!!')
          }
        })
        .catch((error) => {
          _self.$message({
            message: '系統發生錯誤'+error,
            type: 'error',
            duration:0,
            showClose: true,
          })
        })
        .finally(() => loading.close())
    }).catch(() => {
    })
  },
  getshow: function (val) {
    this.isShow = val.show;
  },
  getshow2: function (val) {
    this.isShow2 = val.show;
  },
  getshow3: function (val) {
    this.isShow3 = val.show;
  },
  getcondition: function () {
    var _self = this;
    _self.tableData = []
    _self.total = 0
    apiurl = _self.api_interface.get_data
    this.get_data(apiurl, 111, 1, _self.currentPage, _self.pageSize)
  },
  current_change(val) {
    var _self = this;
    var start = ''
    var end = ''

    start = ((val - 1) * _self.pageSize) + 1;
    end = val * _self.pageSize
    _self.currentPage = val
    _self.tableData = []
    _self.total = 0    
    apiurl = _self.api_interface.get_data
    this.get_data(apiurl, _self.year, _self.sms, start, end)
  },
  size_change(val) {
    var _self = this;
    var start = ''
    var end = ''
    start = ((_self.currentPage - 1) * val) + 1;
    end = _self.currentPage * val
    _self.pageSize = val
    _self.tableData = []
    _self.total = 0      
    apiurl = _self.api_interface.get_data
    this.get_data(apiurl, _self.year, _self.sms, start, end)
  },
  get_data:async function (apiurl, year, sms, start, end) {
    let _self = this
    let class_name = ''
    if(this.queryform.arg_std == ''){
      _self.$message.error('請先點選學生')
      return false
    }

   
    _self.parameter.emp_id = ''      
    _self.parameter.std_no = _self.queryform.arg_std
    _self.parameter.year_id = _self.queryform.year_id
    _self.parameter.sms_id = _self.queryform.sms_id
    _self.parameter.sRowNun = 1
    _self.parameter.eRowNun = 999
    _self.parameter.sch_no = ''
    _self.parameter.is_sys = '0'
    _self.parameter.token = _self.$token

//    const { data, statusText } = await stuAPI.StudCadre.Get(_self.parameter)
    const { data, statusText } = await _self.getUrl(_self.parameter)

    if (statusText !== "OK") {
      throw new Error(statusText);
    }

    if (data.status == 'Y') {     
        _self.total = data.dataset[0].x_total
        
        if(_self.formSaveCheck == 'stuattestationcentraldb'){
          _self.tableData = data.dataset.filter(function(item,index,array){
            return item.x_cnt > 0
          })
        }else{
          _self.tableData = data.dataset  
        }            

        _self.tableData.forEach(function(item,index,array){
                        if(item.zz == 'Y'){                        
                          item.x_status = true
                        }else{  
                          item.x_status = false
                        }                        
                })                                                   
    } else {
        _self.tableData = []
        _self.total = 0
        _self.$message.error('查無資料，請確認')
    }
  },
  getUrl(parameter){
    let class_name = ''
    switch(this.item){
      case 'studiversecheck'://勾選多元學習
              class_name  = ''
              break;
          case 'stucadre'://幹部經歷紀錄
              return stuAPI.StudCadre.GetList(parameter)
              break;
          case 'stustudyf'://彈性學習時間紀錄
              return stuAPI.StuStudyf.GetList(parameter)
              break;
          case 'stugroup'://團體活動時間紀錄
              return stuAPI.StuGroup.GetList(parameter)
              break;
          case 'stucollege'://大學及技專校院先修課程紀錄
              return stuAPI.StuCollege.GetList(parameter)
              break;
          case 'stuworkplace'://職場學習紀錄
              class_name  = ''
              return stuAPI.StuWorkPlace.GetList(parameter)
              break;
          case 'stuother'://其他多元表現紀錄
              class_name  = ''
              return stuAPI.StuOther.GetList(parameter)
              break;
          case 'sturesult'://作品成果紀錄
              class_name  = ''
              return stuAPI.StuResult.GetList(parameter)
              break;
          case 'stuvolunteer'://服務學習紀錄
              class_name  = ''
              return stuAPI.StuVolunteer.GetList(parameter)
              break;
          case 'stulicense'://檢定證照紀錄
              class_name  = 'StuLicense'
              return stuAPI.StuCollege.GetList(parameter)
              break;
          case 'stucompetition'://競賽參與紀錄
              class_name  = ''
              return stuAPI.StuCompetition.GetList(parameter)
              break;
          case 'stuconsult_query'://學生諮詢課程查詢
              class_name  = ''
              break;
          case 'stuattestation'://學生諮詢課程查詢
              class_name  = ''
              break;
          case 'stuattestationconfirm'://確認課程學習成果
              class_name  = ''
              this.flag = 'Y'
              break;
          case 'stuattestationcentraldb'://確認課程學習成果
              class_name  = ''
              break;           
          default:
    }
  }   
},
components: {
  'v-stucadreform': stucdreform,
  'v-stustudyform': stustudyform,
  'v-pubquery': PubQuery,
  'v-stufilelist': stufilelist,
  'v-fileupload': fileupload,
  'v-stugroupform': stugroupform,
  'v-stucollegeform': stucollegeform,
  'v-stuworkplaceform': stuworkplaceform,
  'v-stuotherform': stuotherform,
  'v-sturesultform': sturesultform,
  'v-stuvolunteerform': stuvolunteerform,
  'v-stulicenseform': stulicenseform,
  'v-stucompetitionform':stucompetitionform,
  'v-stuconsultQueryform':stuconsultform,
},
beforeDestroy(){

},
mounted() {

},
beforeMount() {
  let _self = this
  this.opno ='stucadre'
  this.data_structure = data_structure.stu_cadre
  this.data_structure.file_delete = true
  this.$router.options.routes.find(function(item,index,array){
              if(item.name == "stucadre"){
                _self.api_interface = item.props.api_interface
                _self.userStatic = item.props.userStatic
                _self.content = item.props.userStatic.form_component                  
              }
          })
          _self.tableData = []
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
filters:{
  timeString(value,myFormat){
    return moment(value).format(myFormat || 'YYYY-MM-DD, HH:mm:ss')
  }
},
watch:{
  cleanlist(){
    this.tableData = []
  }
}
}
</script>


<style scoped>
.el-main{
  padding: 0px;
}
.el-table__row .hover-row{
  background-color:#ffd04b !important;
}
</style>