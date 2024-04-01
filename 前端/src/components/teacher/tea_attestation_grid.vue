<template>
  <div>
    <div v-if="operate_state.open_yn == 'Y'">

    </div>
    <div v-else>
      <el-alert
        :closable = false
        :title = "'系統開放認證時間：'+ operate_state.startdate + '~' + operate_state.enddate"
        type="error">
      </el-alert>
    </div>     
    <div align="left">
      <div v-if="activePage == '0'">
        <el-button type="primary" v-on:click="batchVerify">批次審核</el-button>
        審核狀態：
        <el-select v-model="passStatus" placeholder="">
          <el-option
                    v-for="item in passStatusList"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value">
          </el-option>
        </el-select>
      </div>
      <div v-if="activePage == '1'">
        <el-button type="primary" v-on:click="batchRelease">批次發佈</el-button>
      </div>           
    </div>
    <el-table :data="tableData" height=67vh stripe style="width: 100%" :row-style="rowState" @row-click="rowClick" v-on:header-click="headerclick">
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
                     
            </div>
        </template>
      </el-table-column>  
      <el-table-column v-for="item in render_header"
        :key="item.prop"
        :prop="item.prop"
        :label="item.label"
        :width="item.width"
        align="center"
      >
      <template slot-scope="scope" >
          <div v-if="item.slot">
            <div v-if="activePage == '0'">
              <div v-if="item.col == 'attestation_status'">
                <el-dropdown split-button type="primary" @command="handleCommand" size="small" :disabled="scope.row.v == 'Y'? true : false">
                  {{scope.row.n == "Y"?'通過':scope.row.n == 'F'?'未通過': '認證中'}}
                  <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item command="Y" @click.native="verify(scope.row)">通過</el-dropdown-item>
                    <el-dropdown-item command="F" @click.native="verify(scope.row)">未通過</el-dropdown-item>
                  </el-dropdown-menu>
                </el-dropdown>   
              </div>  
              <div v-else-if="item.col == 'attestation_release'">
                <div v-if="scope.row.v == 'N' || scope.row.v == '' || scope.row.v == null">
                  <el-tag type="success" effect="dark">未發佈</el-tag>
                </div>
                <div v-else-if="scope.row.v == 'Y'">
                  <el-tag type="primary" effect="dark">已發佈</el-tag>
                </div>   
              </div>   
              <div v-else-if="item.col == 'attestation_reason'">
                <div v-if="scope.row.n == 'F'">
                  <el-popover
                    placement="right"
                    width="400"
                    trigger="click">              
                    <el-select v-model="scope.row.w" placeholder="輸入不通過原因">
                      <el-option
                                v-for="item in attestation_reasonList"
                                :key="item.value"
                                :label="item.label"
                                :value="item.value">
                      </el-option>
                    </el-select>                    
                    <br/>
                    <br/>
                    <el-button type="primary" size="small" v-on:click="attestation_reason_save(scope.row)">存檔</el-button>
                    <!--
<el-button type="info" size="small" @click="attestation_reason_show = false">取消</el-button>
                    -->                    
                    <i :class="scope.row.w ==''|| scope.row.w == null ? 'el-icon-edit':'el-icon-document'" slot="reference" style="cursor:pointer" @click="reason_edit(scope)"></i>                    
                  </el-popover>
                </div>                            
              </div>    
            </div>
            <div v-if="activePage == '1'">
              <div v-if="item.col == 'attestation_status'">
                <div v-if="scope.row.n == 'Y'">
                  <el-tag type="primary" effect="dark">通過</el-tag>  
                </div>
                <div v-else>
                  <el-tag type="danger" effect="dark">未通過</el-tag>  
                </div>                                
              </div>  
              <div v-else-if="item.col == 'attestation_release'">
                <div v-if="scope.row.v == 'N' || scope.row.v == '' || scope.row.v == null">
                  <!--
                    <el-button type="success" size="small" v-on:click="Release(scope.row)">未發佈</el-button>
                  -->                  
                  <el-tag type="success" effect="dark" v-on:click.native="Release(scope.row)" style="cursor:pointer">未發佈</el-tag>
                </div>
                <div v-else-if="scope.row.v == 'Y'">
                  <el-tag type="primary" effect="dark">已發佈</el-tag>
                </div>       
              </div>    
            </div>     
          </div>
          <div v-else>
            {{scope.row[item.prop]}}
          </div>
        </template>      
      </el-table-column>
      <!--
      <el-table-column align="center" fixed="right" label="檢視資料" width="">
        <template slot-scope="scope">
          <i class="el-icon-search" style="cursor:pointer" @click="edit_data(scope)"></i>
        </template>
      </el-table-column>
      <el-table-column align="center" fixed="right" label="上傳檔案" width="">
        <template slot-scope="scope">
          <i class="el-icon-upload" style="cursor:pointer" @click="file_upload(scope)"></i>
        </template>
      </el-table-column>
      -->
      <el-table-column align="center" label="查看檔案" width="">
          <template slot-scope="scope">
            <el-badge :value="scope.row.x_cnt" class="item" type="info" style="cursor:pointer" @click.native="file_data(scope)"></el-badge>
          </template>
      </el-table-column>
      <!--
      <el-table-column style="" align="center" fixed="right" label="刪除資料" width="">
        <template slot-scope="scope">
          <i class="el-icon-delete" style="cursor:pointer" @click="del_data(scope)"></i>
        </template>
      </el-table-column>
      -->
    </el-table>
    <!--
    <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
      v-on:current-change="current_change" v-on:size-change="size_change">
    </el-pagination>
    -->
    <component :is='content' :dialog_show.sync="dialog_show" :edit_model="edit_model" :parameter="parameter"
      :data_structure="data_structure" :userStatic="userStatic" :api_interface="api_interface" v-on:get-show="getshow"
      v-if="isShow">
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
import tea_consult_form from '@/components/teacher/tea_consult_form.vue'
import PubQuery from '@/components/pub/pub_query.vue'
import fileupload from '@/components/pub/pub_upload.vue'
import * as data_structure from '@/js/tea_grid_structure.js'
import * as std_structure from '@/js/stu_grid_structure.js'

export default {
  props: {
    userStatic: {
      type: Object,
    },
    api_interface: {
      type: Object,
    },
    tableData:{
      type:Array
    },
    total:{
      type:Number
    },
    activePage:{
      type:String
    },
    parameter:{
      type: Object,
    },
    operate_state:{
      type: Object,
    },
  },
  data() {
    return {
      content: this.userStatic.form_component,
      data_structure: {},
      //tableData: [],
      isShow: false,
      isShow2: false,
      isShow3: false,
      dialog_show: true,
      dialog_show2: true,
      dialog_show3: true,
      //total: 0,
      currentPage: 1,
      pageSize: 10,
      year: '',
      sms: '',
      edit_model: '',
      filelist: [],
      rowdata: {},
      passStatus:'Y',
      passStatusList:[{value:'Y',label:'通過'},{value:'F',label:'未通過'}],
      passStatusDropDown:'認證中',
      check_list:[],
      attestation_reason:'',
      attestation_reason_show:true,
      attestation_reasonList:[
                                {value:'01',label:'非本課程相關主題成果，無法認證'},
                                {value:'02',label:'無法查證成果可信度'},
                                {value:'03',label:'請加強成果內容正確性'},
                                {value:'04',label:'請增加成果內容豐富度'},
                              ],
      //operate_state:{open_yn:'',startdate:'',enddate:''}
    }
  },
  methods: {
    attestation_reason_save:function(val){
      let _self = this
      let verify = ''
      let formdata = new FormData();
      let temp = val.n
      formdata.append('complex_key[]',val.a +'_'+val.b+'_'+val.c+'_'+val.d+'_'+val.e+'_'+val.f+'_'+val.g+'_'+val.h+'_'+val.i+'@'+val.w)
      formdata.append('token',_self.$token)

      const loading = _self.$loading({
              lock: true,
              text: '資料處理中，請稍後。',
              spinner: 'el-icon-loading',
              background: 'rgba(0, 0, 0, 0.7)'
              });

        const apiurl = _self.api_interface.save_Reason
        _self.$http({
            url:apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.$message.success('存檔成功!!')
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
    attestation_reason_cancel:function(){
      
    },
    reason_edit:function(){

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
    batchVerify:function(){
        var i = 0;
        let _self = this
        let formdata = new FormData();
        let verify = ''

        if(!_self.check_operate()){
          return false
        }

        _self.check_list.length = 0;

        this.tableData.forEach((element, index) => {
          if(element.x_status == true){
            _self.check_list[i++] = element
          }
        });

        if(_self.check_list.length == 0){
          _self.$message.error('未勾選認證學生，請確認!!');
          return false;
        }     

        if(this.passStatus == 'Y'){
           verify = '確定將認證狀態改為通過?'
        }else{
           verify = '確定將認證狀態改為未通過?'
        }

        _self.check_list.forEach(function(value, index, array){
          formdata.append('complex_key[]',value.a +'_'+value.b+'_'+value.c+'_'+value.d+'_'+value.e+'_'+value.f+'_'+value.g+'_'+value.h+'_'+value.i+'@'+_self.passStatus)
        });
         formdata.append('token',_self.$token)

        _self.$confirm(`${verify}`, 'Warning', {
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

        const apiurl = _self.api_interface.save_Verify
        _self.$http({
            url:apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.$message.success('變更成功!!')
                                _self.check_list.forEach(function(value, index, array){
                                  value.n = _self.passStatus
                                });
                               
                            }else{
                                _self.$message.error(res.data.message)
                            }
                    })
            .catch((error) => {
                _self.$message({
                message: '變更失敗:'+error,
                type: 'error',
                duration:0,
                showClose: true,
                })
            })
            .finally(()=>loading.close())      
        }).catch(() => {
        })        
    },
    batchRelease:function(){
      var i = 0;
        let _self = this
        let formdata = new FormData();
        let verify = ''

        if(!_self.check_operate()){
          return false
        }        
        _self.check_list.length = 0;

        this.tableData.forEach((element, index) => {
          if(element.x_status == true){
            _self.check_list[i++] = element
          }
        });

        if(_self.check_list.length == 0){
          _self.$message.error('未勾選發佈學生，請確認!!');
          return false;
        }     

        _self.check_list.forEach(function(value, index, array){
          formdata.append('complex_key[]',value.a +'_'+value.b+'_'+value.c+'_'+value.d+'_'+value.e+'_'+value.f+'_'+value.g+'_'+value.h+'_'+value.i)
        });
        formdata.append('token',_self.$token)

        _self.$confirm(`確認將已勾選的學生認證發佈`, 'Warning', {
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

        const apiurl = _self.api_interface.save_Release
        _self.$http({
            url:apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.$message.success('發佈成功!!')
                            }else{
                                _self.$message.error(res.data.message)
                            }
                    })
            .catch((error) => {
                _self.$message({
                message: '發佈失敗:'+error,
                type: 'error',
                duration:0,
                showClose: true,
                })
            })
            .finally(()=>loading.close())      
        }).catch(() => {
        })        
    },
    Release:function(val){
      let _self = this
      let verify = ''
      let formdata = new FormData();
      let temp = val.n

      if(!_self.check_operate()){
          return false
        }

      formdata.append('complex_key[]',val.a +'_'+val.b+'_'+val.c+'_'+val.d+'_'+val.e+'_'+val.f+'_'+val.g+'_'+val.h+'_'+val.i)
      formdata.append('token',_self.$token)

      this.$confirm(`確認將此學生認證發佈??`, 'Warning', {
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

        const apiurl = _self.api_interface.save_Release
        _self.$http({
            url:apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.$message.success('發佈成功!!')
                            }else{
                                _self.$message.error(res.data.message)
                            }
                    })
            .catch((error) => {
                _self.$message({
                message: '發佈失敗:'+error,
                type: 'error',
                duration:0,
                showClose: true,
                })
            })
            .finally(()=>loading.close())              
      }).catch(() => {        
      })     
    },
    verify:function(val){
      let _self = this
      let verify = ''
      let formdata = new FormData();
      let temp = val.n

      if(!_self.check_operate()){
          return false
        }

      formdata.append('complex_key[]',val.a +'_'+val.b+'_'+val.c+'_'+val.d+'_'+val.e+'_'+val.f+'_'+val.g+'_'+val.h+'_'+val.i+'@'+_self.passStatusDropDown)
      formdata.append('token',_self.$token)

      if(this.passStatusDropDown == 'Y'){
        verify = '確定將認證狀態改為通過?'
      }else{
        verify = '確定將認證狀態改為未通過?'
      }
      this.$confirm(`${verify}`, 'Warning', {
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

        const apiurl = _self.api_interface.save_Verify
        _self.$http({
            url:apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                                _self.$message.success('變更成功!!')
                                val.n = _self.passStatusDropDown
                            }else{
                                _self.$message.error(res.data.message)
                            }
                    })
            .catch((error) => {
                _self.$message({
                message: '變更失敗:'+error,
                type: 'error',
                duration:0,
                showClose: true,
                })
            })
            .finally(()=>loading.close())              
      }).catch(() => {
        if(temp == ''){
          val.n = ''
        }
        
      })     
    },
    handleCommand(command){
      this.passStatusDropDown = command
      //this.passStatusDropDown = command == 'Y' ? '通過' : '未通過'
    },
    rowClick(row,column,event){
        this.$emit('get-studata', row)
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

      _self.parameter["flag"] = _self.userStatic.data_structure
      _self.parameter["class_name"] = _self.userStatic.interface
      //_self.parameter.sch_no = '190406'
      _self.parameter.sRowNun = 1
      _self.parameter.eRowNun = 999
       complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}_${val.row.f}_${val.row.g}_${val.row.h}_0`
       _self.parameter["complex_key"] = complex_key
       _self.parameter["token"] = _self.$token

      _self.$http({
        url: apiurl,
        method: 'get',
        params: _self.parameter,
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {
          if (res.data.status == 'Y') {
            _self.filelist = res.data.dataset
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
        formdata.append('token',_self.$token)
        
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
    getcondition: function (val) {
      var _self = this;
      _self.year = val.year
      _self.sms = val.sms

      apiurl = _self.api_interface.get_data
      this.get_data(apiurl, val.year, val.sms, _self.currentPage, _self.pageSize)
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
    get_data: function (apiurl, year, sms, start, end) {
      let _self = this
      const loading = _self.$loading({
        lock: true,
        text: '資料讀取中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
      });

      _self.parameter.emp_id = ''
      _self.parameter.year_id = year
      _self.parameter.sms_id = sms
      _self.parameter.sRowNun = start
      _self.parameter.eRowNun = end
      _self.parameter.sch_no = ''
      _self.parameter.token = _self.$token

      _self.$http({
        url: apiurl,
        method: 'get',
        params: _self.parameter,
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {
          if (res.data.status == 'Y') {
            _self.tableData = res.data.dataset                
            _self.total = res.data.dataset[0].x_cnt
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
    check_operate:function(){
      if(this.operate_state.open_yn == 'N'){
        this.$message.warning ("已超過開放認證時間，請確認!!")
        return false
      }
      return true
    }
  },
  components: {
    'v-teaconsultform': tea_consult_form,
    'v-pubquery': PubQuery,
    'v-stufilelist': stufilelist,
    'v-fileupload': fileupload,
  },
  beforeDestroy(){

  },
  mounted() {
 
  },
  beforeMount() {
    switch (this.userStatic.data_structure) {
            case 'teaattestation'://授課教師認證課程學習成果
              this.data_structure = data_structure.tea_attestation
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
    tableData:function(val1,val2){

    }
  }
}
</script>

<style>
.item {
  margin-top: 10px;
}
</style>
