<template>
  <div>
    <v-pubquery v-on:get-condition="getcondition"></v-pubquery>
    <div align="left" v-if="data_structure.course_select" style="margin-bottom: 10px;">
      <el-button type="warning">新增認證課程</el-button>
    </div>    
    <div align="left" v-if="data_structure.add_data" style="margin-bottom: 10px;">
      <el-button type="primary" v-on:click="add">新增</el-button>
    </div>
    <!--
    <div align="left" v-if="data_structure.confirm_batch">
      <el-button type="primary" v-on:click="confirm_batch">確認送出</el-button>
    </div>
    -->
    <div align="left" v-if="data_structure.centraldb_batch">
      <div v-if="operate_state.open_yn == 'Y'">
        <el-button type="primary" v-on:click="centraldb_batch">確認存檔</el-button>
      </div>
      <div v-else>
        <el-alert
          :closable = false
          :title = "'系統開放勾選時間：'+ operate_state.startdate + '~' + operate_state.enddate"
          type="error">
        </el-alert>
      </div>      
    </div>  
    <div align="left" v-if="this.formSaveCheck == 'stuattestation'">
      <div v-if="operate_state.open_yn == 'Y'">
        
      </div>
      <div v-else>
        <el-alert
          :closable = false
          :title = "'系統開放認證及上傳時間：'+ operate_state.startdate + '~' + operate_state.enddate"
          type="error">
        </el-alert>
      </div> 
    </div>  
    <el-table :data="tableData" height=70vh stripe style="width: 100%" :row-style="rowState" v-on:header-click="headerclick">
      <el-table-column type="index" width="50"></el-table-column>     
      <el-table-column prop="x_status"
              width="80"
              header-align="center"
              align="center"
              label="全選"
              style="cursor:pointer" v-if="data_structure.checkbox"
              >
        <template slot-scope="scope">                          
            <div v-if="scope.row.x_status == '' || scope.row.x_status == true || scope.row.x_status == false" >
              <el-checkbox  v-model="scope.row.x_status"></el-checkbox>             
            </div>
            <div v-else>
              <el-tag type="primary" effect="dark">{{scope.row.x_status}}</el-tag>                
            </div>
        </template>
      </el-table-column>  
      <el-table-column  v-for="item in render_header"
      :key="item.prop"
      :prop="item.prop"
      :label="item.label"
      :width="item.width"   
      align="center">
      >
        <template slot-scope="scope" >
          <div v-if="item.slot">
            <div v-if="item.col == 'attestation_send'">
              <div v-if="scope.row.l == '' || scope.row.l == null || (scope.row.n == 'F' && scope.row.x == 'Y')">
                <el-button type="primary" size="small " v-on:click="send(scope)">送出認證</el-button>        
              </div>
              <div v-else>
                {{scope.row[item.prop]}}
              </div>     
            </div>
            <div v-else-if="item.col == 'attestation_centraldb'">
              <div align="center">
                <el-checkbox v-model="scope.row.v"></el-checkbox> 
              </div>
            </div>            
            <div v-else>          
              <!--
                <div v-if="scope.row.n == 'N' && (scope.row.x == '' || scope.row.x == null) ">
              -->
              <div v-if="(scope.row.n == 'N' || scope.row.n == 'Y' || scope.row.n == 'F') && (scope.row.x == '' || scope.row.x == null) ">
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
      <el-table-column align="center" fixed="right" label="檢視資料" width="" v-if="data_structure.query_data">
        <template slot-scope="scope">
          <i class="el-icon-search" style="cursor:pointer" @click="edit_data(scope)"></i>
        </template>
      </el-table-column>
      <el-table-column align="center" fixed="right" label="上傳檔案" width="" v-if="data_structure.upload_file">
        <template slot-scope="scope">
          <i class="el-icon-upload" style="cursor:pointer" @click="file_upload(scope)"></i>
        </template>
      </el-table-column>
      <el-table-column align="center" fixed="right" label="可上傳檔案數" width="" v-if="this.formSaveCheck == 'stuattestation'">
          <template slot-scope="scope">
            <el-badge :value="scope.row.x_file_cnt - scope.row.x_cnt" class="item" type="primary"></el-badge>
          </template>
      </el-table-column>  
      <el-table-column align="center" fixed="right" label="已勾選確認檔案數" width="" v-if="this.userStatic.data_structure == 'stuattestationconfirm'">
          <template slot-scope="scope">
            <el-badge :value="scope.row.x_file_center_cnt" class="item" type="primary"></el-badge>
          </template>
      </el-table-column>            
      <el-table-column align="center" fixed="right" label="查看檔案" width="" v-if="data_structure.download_file">
          <template slot-scope="scope">
            <el-badge :value="scope.row.x_cnt" class="item" type="info" style="cursor:pointer" @click.native="file_data(scope)"></el-badge>
            <!--
                <i class="el-icon-s-order" style="cursor:pointer" @click="file_data(scope)"></i>
            -->
          </template>
      </el-table-column>
      <el-table-column style="" align="center" fixed="right" label="刪除資料" width="" v-if="data_structure.delete_data">
        <template slot-scope="scope">
          <i class="el-icon-delete" style="cursor:pointer" @click="del_data(scope)"></i>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination  :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
      v-on:current-change="current_change" v-on:size-change="size_change">
    </el-pagination>

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
import * as fileAPI from  '@/apis/fileApi.js' 
export default {
  props: {
    userStatic: {
      type: Object,
    },
    api_interface: {
      type: Object,
    },
  },
  data() {
    return {
      formSaveCheck:'',
      content: this.userStatic.form_component,
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
      datarow:{},
      operate_state:{open_yn:'Y',startdate:'',enddate:''}
    }
  },
  methods: {
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
            formdata.append('centraldb[]',value.v)
            formdata.append('complex_key[]',value.a +'_'+value.b+'_'+value.c+'_'+value.d+'_'+value.e+'_'+value.f+'_'+value.g+'_'+value.h+'_'+value.i+'@'+value.v)
          });

        formdata.append('token',_self.$token)
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
        formdata.append('token',_self.$token)

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

      if(this.operate_state.open_yn == 'N'){
        _self.$message.warning ("已超過開放認證時間，請確認!!")
        return false;      
      }

      if(val.row.x_cnt == 0){
        _self.$message.warning ("請先上傳檔案後，再執行送出認證。")
        return false;
      }

      this.data_structure.header.forEach(function(value, index, array){
              formdata.append(value.col,val.row[value.prop]);
          });
          
      formdata.append("token",_self.$token)
       
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
      return {
        backgroundColor: '#f4f4f5',
      }
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
    file_data: async function (val) {
      this.datarow = val
      let _self = this
      let complex_key = ''
      apiurl = _self.api_interface.file_data

      _self.data_structure.header.forEach(function (value, index, array) {
        if (value.parameter == "Y") {
          _self.parameter[value.col] = val.row[value.prop]
        }
      });

      _self.parameter["class_name"] = _self.userStatic.interface

      if(_self.userStatic.interface == 'StuAttestation'){
     
        if(val.row.n == 'N' || val.row.n == 'Y'){
          _self.userStatic.file_delete = false
          _self.parameter["deleteFile"] = 'N'
        }
        else{
          if(_self.formSaveCheck == 'stuattestationconfirm')
          {
            _self.userStatic.file_delete = false
            _self.parameter["deleteFile"] = 'N'
          }
          else
          {
            _self.userStatic.file_delete = true
            _self.parameter["deleteFile"] = 'Y'
          }

        }        
      }
      _self.userStatic.checkbox = false

      _self.parameter["flag"] = _self.userStatic.data_structure
      if(_self.userStatic.interface == 'StuAttestation'){
            complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}_${val.row.f}_${val.row.g}_${val.row.h}_0`
      }else if(_self.userStatic.interface == 'StudCadre'){
          complex_key = `${val.row.a}_${val.row.c}_${val.row.d}_${val.row.b}_${val.row.k}`                   
      }
      else{
          complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}`
      }

      _self.parameter["complex_key"] = complex_key
      _self.parameter["token"] = _self.$token

      const { data, statusText } = await fileAPI.StuFileInfo.file_data(_self.parameter) 

      if (statusText !== "OK") {
        throw new Error(statusText);
      }

      if (data.status == 'Y') {    
          if(_self.formSaveCheck == 'stuattestationcentraldb'){
            _self.filelist = data.dataset.filter(function(item,index,array){
              return item.attestation_file_yn == 'Y'
            })
          }
          else{
            _self.filelist = data.dataset  
          } 
        
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

      this.isShow2 = true
    },
    file_upload: function (val) {
      this.datarow = val
      let _self = this

      if(_self.formSaveCheck == 'stuattestation') {
        if(val.row.x_file_cnt - val.row.x_cnt <= 0){
          _self.$message.warning ("已達上傳檔案上限，請確認!!")
          return false;
        }

        if(this.operate_state.open_yn == 'N'){
          _self.$message.warning ("已超過開放上傳檔案時間，請確認!!")
          return false;      
        }        
      }     

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
      _self.parameter["token"] = _self.$token
      this.isShow3 = true
    },
    del_data:async function (val) {
      let _self = this
      apiurl = _self.api_interface.del_data
      if (this.data_structure.delete_rule.rule_flag === "Y") {
        if (!this.data_structure.delete_rule.rule_check(val, _self)) {
          return false
        }
      }

      let formdata = new FormData();

      _self.data_structure.header.forEach(function (value, index, array) {
        if (value.parameter == "Y") {
          _self.parameter[value.col] = val.row[value.prop]
          formdata.append(value.col, val.row[value.prop]);
        }
      });

      formdata.append("token",_self.$token)
      formdata.append("class_name",_self.userStatic.interface)

      const confirm =  await _self.$confirm(`確定刪除?`, 'Warning', {
                confirmButtonText: '確定',
                cancelButtonText: '取消',
                type: 'warning'
              }).catch(()=>{})

      if(confirm !== 'confirm')
      {
        return  _self.$message.info('已取消刪除!!')
      }

      const { data, statusText } = await apiurl(formdata) 

      if (statusText !== "OK") {
        throw new Error(statusText);
      }

      if (data.status == 'Y') {
          _self.$message.success('刪除成功!!')
          _self.tableData.splice(val.$index, 1);
      } else {
        _self.$message.error('刪除失敗!!')
      }      
    },
    getshow: function (val) {
      this.isShow = val.show;
    },
    getshow2: function (val) {
      if(this.formSaveCheck == 'stuattestationcentraldb'){
        this.tableData[this.datarow.$index].x_file_center_cnt = Number(val.file_cnt)
      }
      this.isShow2 = val.show;
    },
    getshow3: function (val) {    
      let cnt = this.tableData[this.datarow.$index].x_cnt
      let filename = this.tableData[this.datarow.$index].s

      
      if(val.success){
        cnt = cnt == '' || cnt == null ? 1 : cnt = cnt +1
        filename = filename == '' || filename == null ? val.filename :filename = filename +','+val.filename
        this.tableData[this.datarow.$index].x_cnt = Number(cnt)
        this.tableData[this.datarow.$index].s = filename
      }  
      this.isShow3 = val.show;
    },
    getcondition: function (val) {
      var _self = this;
      _self.year = val.year
      _self.sms = val.sms

      apiurl = _self.api_interface.get_data
      this.get_data(apiurl, val.year, val.sms, _self.currentPage, _self.pageSize)

      if(this.userStatic.data_structure == 'stuattestationcentraldb')
      {
        apiurl = this.api_interface.get_OpOpenYN
        var parameter={year_id:val.year,sms_id:0,grade_id:'1',type_id:'03',token:this.$token,kind_id:'stu'}
        this.operate_state = data_structure.getOpenOpYN(this,apiurl,parameter,this.$token)  
      }
    },
    current_change(val) {
      var _self = this;
      var start = ''
      var end = ''

      start = ((val - 1) * _self.pageSize) + 1;
      end = val * _self.pageSize
      _self.currentPage = val
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
      apiurl = _self.api_interface.get_data
      this.get_data(apiurl, _self.year, _self.sms, start, end)
    },
    get_data: async function (apiurl, year, sms, start, end) {
      try {
            let _self = this

            _self.parameter.emp_id = ''      
            _self.parameter.std_no = ''
            _self.parameter.year_id = year
            _self.parameter.sms_id = sms
            _self.parameter.sRowNun = start
            _self.parameter.eRowNun = end
            _self.parameter.sch_no = ''
            _self.parameter.is_sys = '0'
            _self.parameter.token = _self.$token

            const { data, statusText } = await apiurl(_self.parameter) 

            if (statusText !== "OK") {
              throw new Error(statusText);
            }

            if (data.status == 'Y') {         
                  if(_self.formSaveCheck == 'stuattestationcentraldb'){
                    _self.tableData = data.dataset.filter(function(item,index,array){
                      return item.x_cnt > 0
                    })
                  }else{
                    _self.tableData = data.dataset  
                  }

                  _self.tableData.forEach(function(item,index,array){
                      if(_self.data_structure.confirm_batch){
                          if(item.u == 'Y'){
                              item.x_status = "已送出"
                            }else{
                              item.x_status = false
                            }
                      }
                      if(_self.data_structure.centraldb_batch){
                          if(item.v == 'Y'){
                              item.v = true
                            }else{
                              item.v = false
                            }
                      }
                      
                  })      
                        
                  _self.total = data.dataset[0].x_total
                  if( _self.tableData.length <= 0){
                    _self.$message.error('查無資料，請確認')
                  }
            } else {
              _self.tableData = []
              _self.$message.error('查無資料，請確認')
            }        
      } catch (error) {
        
      }
    },
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
    let parameter = ''
    switch (this.userStatic.data_structure) {  
      case 'stucadre'://幹部經歷紀錄
        this.data_structure = data_structure.stu_cadre
        this.formSaveCheck = 'stucadre'
        break;
      case 'stustudyf'://彈性學習時間紀錄
        this.data_structure = data_structure.stu_study_f
        this.formSaveCheck = 'stustudyf'        
        break;
      case 'stugroup'://團體活動時間紀錄
        this.data_structure = data_structure.stu_group
        this.formSaveCheck = 'stugroup'        
        break;
      case 'stucollege'://大學及技專校院先修課程紀錄
        this.data_structure = data_structure.stu_college
        this.formSaveCheck = 'stucollege'        
        break;
      case 'stuworkplace'://職場學習紀錄
        this.data_structure = data_structure.stu_workplace
        this.formSaveCheck = 'stuworkplace'        
        break;
      case 'stuother'://其他多元表現紀錄
        this.data_structure = data_structure.stu_other
        this.formSaveCheck = 'stuother'        
        break;
      case 'sturesult'://作品成果紀錄
        this.data_structure = data_structure.stu_result
        this.formSaveCheck = 'sturesult'        
        break;
      case 'stuvolunteer'://服務學習紀錄
        this.data_structure = data_structure.stu_volunteer
        this.formSaveCheck = 'stuvolunteer'        
        break;
      case 'stulicense'://檢定證照紀錄
        this.data_structure = data_structure.stu_license
        this.formSaveCheck = 'stulicense'        
        break;
      case 'stucompetition'://競賽參與紀錄
        this.data_structure = data_structure.stu_competition
        this.formSaveCheck = 'stucompetition'        
        break;
      case 'stuconsult_query'://學生諮詢課程查詢
        this.data_structure = data_structure.consult_query
        this.formSaveCheck = 'stuconsult_query'        
        break;
      case 'stuattestation'://學生諮詢課程查詢
        this.data_structure = data_structure.stu_attestation
        this.formSaveCheck = 'stuattestation'     
        apiurl = this.api_interface.get_OpOpenYN
        parameter={year_id:111,sms_id:1,grade_id:'1',type_id:'01',token:this.$token,kind_id:'stu'}
        this.operate_state = data_structure.getOpenOpYN(this,apiurl,parameter,this.$token)           
        break;
      case 'stuattestationconfirm'://確認課程學習成果
        this.data_structure = data_structure.stu_attestation_confirm
        this.formSaveCheck = 'stuattestationconfirm'        
        break;
      case 'stuattestationcentraldb'://確認課程學習成果
        this.data_structure = data_structure.stu_attestation_centraldb
        this.formSaveCheck = 'stuattestationcentraldb'      
        //apiurl = this.api_interface.get_OpOpenYN
        //parameter={year_id:111,sms_id:0,grade_id:'1',type_id:'03',token:this.$token,kind_id:'stu'}
        //this.operate_state = data_structure.getOpenOpYN(this,apiurl,parameter,this.$token)               
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
  filters:{
    timeString(value,myFormat){
      return moment(value).format(myFormat || 'YYYY-MM-DD, HH:mm:ss')
    }
  }
}
</script>

<style>
.item {
  margin-top: 10px;
}
</style>
