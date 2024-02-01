<template>
    <div>
        <div align="left">
          <v-pubquery v-on:get-condition="getcondition"></v-pubquery>
        </div> 
        <div align="left">
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
        <div class="container">
            <div class="item-right" style="width:25%;height:100%;">
                <el-table 
                          :data="leftList"  
                          height=70vh  
                          stripe 
                          :row-class-name="rowclassname"
                          :cell-style="cellstyle"
                          :row-style="leftrowState" 
                          v-on:row-click="rowclick"
                          :cell-class-name="classChecker"
                          v-on:cell-click="cellclick">
                    <el-table-column type="index" width="50"></el-table-column>
                    <el-table-column                      
                      prop="diverseid"
                      :label=leftTitle>
                    </el-table-column>
                </el-table>
            </div>
            <div class="item-left"  style="width:75%;height:100%;">
                <el-table :data="tableData"  height=70vh  stripe  :row-style="rowState" v-on:header-click="headerclick">
                    <el-table-column type="index" width="50"></el-table-column>     
                    <el-table-column prop="x_status"
                            width="80"
                            header-align="center"
                            align="center"
                            label="全選"
                            style="cursor:pointer"
                            >
                        <template slot-scope="scope">                                               
                          <el-checkbox  v-model="scope.row.x_status" v-if="scope.row.x_cnt > 0"></el-checkbox>      
                        </template>
                    </el-table-column>  
                    <el-table-column  v-for="item in render_header"
                    :key="item.prop"
                    :prop="item.prop"
                    :label="item.label"
                    :width="item.width"   
                    align="center">
                    </el-table-column>
                    <el-table-column align="center"  label="檢視資料" width="" v-if="data_structure.query_data">
                        <template slot-scope="scope">
                        <i class="el-icon-search" style="cursor:pointer" @click="edit_data(scope)"></i>
                        </template>
                    </el-table-column>
                    <!--
                    <el-table-column align="center" fixed="right" label="上傳檔案" width="" v-if="data_structure.upload_file">
                        <template slot-scope="scope">
                        <i class="el-icon-upload" style="cursor:pointer" @click="file_upload(scope)"></i>
                        </template>
                    </el-table-column>
                    -->
                    <el-table-column align="center" label="查看檔案" width="" v-if="data_structure.download_file">
                        <template slot-scope="scope">
                            <el-badge :value="scope.row.x_cnt" class="item" type="info" style="cursor:pointer" @click.native="file_data(scope)"></el-badge>
                            <!--
                                <i class="el-icon-s-order" style="cursor:pointer" @click="file_data(scope)"></i>
                            -->
                        </template>
                    </el-table-column>
                    <!--
                    <el-table-column style="" align="center" fixed="right" label="刪除資料" width="" v-if="data_structure.delete_data">
                        <template slot-scope="scope">
                        <i class="el-icon-delete" style="cursor:pointer" @click="del_data(scope)"></i>
                        </template>
                    </el-table-column>
                    -->
                </el-table>
                <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
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
        </div>        
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

export default {
  props: {

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
                    {diverseid:'幹部經歷紀錄',width:'',prop:'',col:'stucadre',parameter:'N',hidden:'N',sort:1,defult:'',slot:false},
                    {diverseid:'競賽參與紀錄',width:'',prop:'',col:'stucompetition',parameter:'N',hidden:'N',sort:2,defult:'',slot:false},
                    {diverseid:'檢定證照紀錄',width:'',prop:'',col:'stulicense',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
                    {diverseid:'檢視志工服務紀錄',width:'',prop:'',col:'stuvolunteer',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
                    {diverseid:'作品成果紀錄',width:'',prop:'',col:'sturesult',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
                    {diverseid:'其他活動紀錄',width:'',prop:'',col:'stuother',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
                    {diverseid:'彈性學習紀錄',width:'',prop:'',col:'stustudyf',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},
                    {diverseid:'職場學習紀錄',width:'',prop:'',col:'stuworkplace',parameter:'N',hidden:'N',sort:8,defult:'',slot:false},
                    {diverseid:'大學及技專校院先修課程紀錄',width:'',prop:'',col:'stucollege',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
                    {diverseid:'團體活動時間紀錄',width:'',prop:'',col:'stugroup',parameter:'N',hidden:'N',sort:10,defult:'',slot:false},   
                ],
      leftTemp:[
          {diverseid:'幹部經歷紀錄',width:'',prop:'',col:'stucadre',parameter:'N',hidden:'N',sort:1,defult:'',slot:false},
          {diverseid:'競賽參與紀錄',width:'',prop:'',col:'stucompetition',parameter:'N',hidden:'N',sort:2,defult:'',slot:false},
          {diverseid:'檢定證照紀錄',width:'',prop:'',col:'stulicense',parameter:'N',hidden:'N',sort:3,defult:'',slot:false},
          {diverseid:'檢視志工服務紀錄',width:'',prop:'',col:'stuvolunteer',parameter:'N',hidden:'N',sort:4,defult:'',slot:false},
          {diverseid:'作品成果紀錄',width:'',prop:'',col:'sturesult',parameter:'N',hidden:'N',sort:5,defult:'',slot:false},
          {diverseid:'其他活動紀錄',width:'',prop:'',col:'stuother',parameter:'N',hidden:'N',sort:6,defult:'',slot:false},
          {diverseid:'彈性學習紀錄',width:'',prop:'',col:'stustudyf',parameter:'N',hidden:'N',sort:7,defult:'',slot:false},
          {diverseid:'職場學習紀錄',width:'',prop:'',col:'stuworkplace',parameter:'N',hidden:'N',sort:8,defult:'',slot:false},
          {diverseid:'大學及技專校院先修課程紀錄',width:'',prop:'',col:'stucollege',parameter:'N',hidden:'N',sort:9,defult:'',slot:false},
          {diverseid:'團體活動時間紀錄',width:'',prop:'',col:'stugroup',parameter:'N',hidden:'N',sort:10,defult:'',slot:false},   
      ],                
        queryform:{
            year_id:"",
            sms_id:"",
            cls_id:'',  
            std_no:'',
            std_name:'',
            emp_id:"",
            sRowNun:1,
            eRowNun:10    
      },
      yearlist:[],
      operate_state:{open_yn:'Y',startdate:'',enddate:''},
      color:"#606266",
      inlineClass: "color:red",
      getIndex:1,
      leftTitle:"多元學習項目",
      diverse_sel:0,
      diverse_tot:0,
      x_1:0,
      x_2:0,
      x_3:0,
      x_4:0,
      x_5:0,
      x_6:0,
      x_7:0,
      x_8:0,
      x_9:0,
      x_10:0,
      is_studiversecheck:"N"
    }
  },
  methods: {
    check_data:function(data)
    {

    },
    query:function(){
        
    },
    yearChange:function(){

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
                                        }else{
                                          _self.queryform.year_id = ""
                                          _self.yearlist = []
                                        }  
                                    })         
                                  .catch((error)=>{
                                            _self.$message.error('呼叫後端【s90yearinfo】發生錯誤,'+error)
                                          })
                                  .finally()                             
                        });      
    },    
    rowclassname:function(row,rowindex){

    },
    cellstyle:function(row, column, rowIndex, columnIndex){
    },
    classChecker({ row, column }) {
    },    
    cellclick:function(row, column, cell, event){

    },
    rowclick:function(row, column, event){
        this.getIndex = row.sort
        let _self = this
        _self.opno = row.col
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
                  _self.content = item.props.userStatic.form_component
                }
            })
            _self.tableData = []
            /*
            var obj = {
                        year:_self.year,
                        sms: _self.sms
                      }
                      
            _self.getcondition(obj)*/
    },
    centraldb_batch:function(){
          var i = 0;
          var no = 0;
          let _self = this
          let formdata = new FormData();

          _self.check_list.length = 0;

          this.tableData.forEach((element, index) => {
            _self.check_list[i++] = element
            if(element.x_status)
            {
              no++;
            }
          });

          if((this.diverse_sel - this['x_'+this.getIndex])+no > this.diverse_tot)
          {
            _self.$message.error('已大於上限10筆，請確認!!')
            return false
          }
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

        formdata.append('token',_self.$token)
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
      _self.parameter.token = _self.$token
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
    leftrowState(row, rowindex){
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
    rowState(row, rowindex) {
      return {
          backgroundColor: '#f4f4f5',
          cursor:'pointer',
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

      _self.parameter["class_name"] = _self.userStatic.interface

      if(_self.userStatic.interface == 'StuAttestation'){
        if(val.row.n == 'N' || val.row.n == 'Y'){
          _self.userStatic.file_delete = false
          _self.parameter["deleteFile"] = 'N'
        }
        else{
          _self.userStatic.file_delete = true
          _self.parameter["deleteFile"] = 'Y'
        }        
      }

      if(_self.userStatic.interface == 'StuAttestation'){
            complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}_${val.row.f}_${val.row.g}_${val.row.h}_0`
        }else if(_self.userStatic.interface == 'StudCadre'){
            complex_key = `${val.row.a}_${val.row.c}_${val.row.d}_${val.row.b}_${val.row.e}_${val.row.h}`                    
        }
        else{
            complex_key = `${val.row.a}_${val.row.b}_${val.row.c}_${val.row.d}_${val.row.e}`
        }

       _self.parameter["complex_key"] = complex_key
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
    getcondition: function (val) {
      var _self = this;
      _self.year = val.year
      _self.sms = val.sms
      _self.tableData = []
      _self.total = 0
      apiurl = _self.api_interface.get_data
      this.get_data(apiurl, val.year, val.sms, _self.currentPage, _self.pageSize)
      this.$router.options.routes.find(function(item,index,array){
                if(item.name == "studiversecheck"){
                  apiurl = item.props.api_interface.get_OpOpenYN
                  let parameter={year_id:val.year,sms_id:0,grade_id:'1',type_id:'04',token:_self.$token,kind_id:'stu'}
                  _self.operate_state = data_structure.getOpenOpYN(_self,apiurl,parameter,_self.$token)                                    
                }
            })
    },
    current_change(val) {
      var _self = this;
      var start = ''
      var end = ''

      start = ((val - 1) * _self.pageSize) + 1;
      end = val * _self.pageSize
      /*
      _self.currentPage = val
      _self.tableData = []
      _self.total = 0      */
      apiurl = _self.api_interface.get_data
      this.get_data(apiurl, _self.year, _self.sms, start, end)   
    },
    size_change(val) {
      var _self = this;
      var start = ''
      var end = ''
      start = ((_self.currentPage - 1) * val) + 1;
      end = _self.currentPage * val
      /*
      _self.pageSize = val
      _self.tableData = []
      _self.total = 0 */
      apiurl = _self.api_interface.g     
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
      _self.parameter.std_no = ''
      _self.parameter.token = _self.$token
      _self.parameter.year_id = year
      _self.parameter.sms_id = sms
      _self.parameter.sRowNun = start
      _self.parameter.eRowNun = end
      _self.parameter.sch_no = ''
      _self.parameter.is_sys = '2'

      _self.$http({
        url: apiurl,
        method: 'get',
        params: _self.parameter,
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {  
          if (res.data.status == 'Y') {     
            if(_self.opno == 'stucadre'){
              _self.tableData = res.data.dataset.filter(function(item,index,array){
                return item.j == '2'
              })
            }else{
              _self.tableData = res.data.dataset  
            }
            
            _self.tableData.forEach(function(item,index,array){
                            if(item.zz == 'Y'){                        
                              item.x_status = true
                            }else{  
                              item.x_status = false
                            }                        
                    })                       
            _self.total = res.data.dataset[0].x_total
          } else {
            _self.tableData = []
            _self.total = 0
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
        .finally(() => {
          loading.close()
          _self.get_diverse_total()
        })
    },
    get_diverse_total:function()
    {
      let _self = this
      var obj = {
                    year_id:_self.year,
                    sms_id: _self.sms
                }
    apiurl = `${_self.$apiroot}/Get_Diverse_Total`      
    _self.$http({
        url: apiurl,
        method: 'get',
        params: obj,
        headers:{'SkyGet':_self.$token}
      })
        .then((res) => {  
         if(res.data.status == 'Y')
         {
          _self.leftList.forEach(function(value, index, array){ 
            value.diverseid = _self.leftTemp[index].diverseid
            _self['x_'+(index+1)] = res.data.dataset[0]['x_'+(index+1)] 
            value.diverseid = `${value.diverseid} [已勾選${res.data.dataset[0]['x_'+(index+1)]}/共${res.data.dataset[0]['x_'+(index+1)+'_tot']}筆]`
          })

          _self.leftTitle = '多元學習項目'
          _self.diverse_sel = res.data.dataset[0].x_1+
                              res.data.dataset[0].x_2+
                              res.data.dataset[0].x_3+
                              res.data.dataset[0].x_4+
                              res.data.dataset[0].x_5+
                              res.data.dataset[0].x_6+
                              res.data.dataset[0].x_7+
                              res.data.dataset[0].x_8+
                              res.data.dataset[0].x_9+
                              res.data.dataset[0].x_10

          _self.diverse_tot = res.data.dataset[0].x_total
          _self.leftTitle = `${_self.leftTitle}[已勾選${_self.diverse_sel}筆/最多${_self.diverse_tot}筆]`
         }
         else
         {
          _self.diverse_sel = 0
          _self.diverse_tot = 0
          _self.leftTitle = '多元學習項目'
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
        .finally()
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
    //this.getyearlist()
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
    /*
    this.$router.options.routes.find(function(item,index,array){
                if(item.name == "studiversecheck"){
                  apiurl = item.props.api_interface.get_OpOpenYN
                  let parameter={year_id:111,sms_id:0,grade_id:'1',type_id:'04',token:_self.$token,kind_id:'stu'}
                  _self.operate_state = data_structure.getOpenOpYN(_self,apiurl,parameter,_self.$token)                                    
                }
            })*/
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

<style scoped>
.container{
   display:flex; /*一開始需要在父層宣告為 flex 才有效*/
}
​
.item-right{
   flex:1; /*對應子層加上；1數值是內元件之間佔的比例*/
}
​
.item-left{
   flex:1; /*對應子層加上*/
}.red {
  color: red;
}

.bold {
  font-weight: bold;
}
</style>