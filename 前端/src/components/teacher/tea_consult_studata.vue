<template>
  <div>
      <el-dialog title="學生列表"
          :visible.sync="dialogFormVisible"
          v-on:close="close"
          :fullscreen="false" width="100%"
          :close-on-press-escape="false" :close-on-click-modal="false">
          <el-row>
            <el-col :span="24">
              <el-form :model="form" :inline="true" >
                <el-form-item label="班級:" size="mini">
                  <el-select v-model="form.cls_id" placeholder="">
                      <el-option v-for="item in clslist" :key="item.cls_id" :label="item.cls_abr" :value="item.cls_id" >
                      </el-option>
                  </el-select>
                </el-form-item>
                <el-form-item label="學生姓名：" size="mini">
                  <el-input placeholder="輸入關鍵字篩選"
                        v-model="form.std_name"
                        style="width:200px;">
                  </el-input>
                </el-form-item>
                <el-form-item label="" size="mini">
                  <el-button type="primary" plain v-on:click="query" size="mini">查詢</el-button>
                </el-form-item>
              </el-form>
            </el-col>
          </el-row>
          <el-row>
				    <el-col :span="24">
              <el-table :data="stulist" height="50vh" stripe style="width: 100%" :row-style="rowState" v-on:header-click="headerclick">
                <!--
                  <el-table-column type="index" width="50"></el-table-column>
                -->
                
                <el-table-column prop="x_status"
                  width="80"
                  header-align="center"
                  align="center"
                  label="全選"
                  >
                  <template slot-scope="scope">
                      <el-checkbox v-model="scope.row.x_status"></el-checkbox>
                  </template>
                </el-table-column>
                <el-table-column v-for="item in render_header"
                  :key="item.prop"
                  :prop="item.prop"
                  :label="item.label"
                  :width="item.width"
                >
                </el-table-column>
              </el-table>
              <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
                v-on:current-change="current_change" v-on:size-change="size_change">
              </el-pagination>
            </el-col>
          </el-row>
          <br/>
          <br/>
          <div align="right">
              <el-button type="primary" @click="save">新增諮詢學生</el-button>
              <el-button type="primary" @click="cancel">關閉視窗</el-button>
          </div>
      </el-dialog>
  </div>
</template>

<script>
  export default {
          props: {
                  dialog_show: {
                      type: Boolean
                  },
                  filelist:{
                      type:Array
                  },
                  userStatic:{
                      type:Object
                  },
                  parameter:{
                      type:Object
                  },
                  api_interface:{
                      type:Object
                  },
                  data_structure:{
                    type:Object
                  },
                  tea_consult:{
                    type:Object
                  }
          },
          data() {
              return {
                  dialogFormVisible: this.dialog_show,
                  total:0,
                  currentPage: 1,
                  pageSize: 10,
                  stulist:[],
                  clslist:[],
                  check_list:[],
                  form:{
                        cls_id:'',
                        std_name:'',
                        year_id:'',
                        sms_id:'',
                        emp_id:'',
                        sch_no:'',
                        sRowNun:1,
                        eRowNun:10,
                        x_status:'',cls_id_q:'',std_name_q:'',token:this.$token                    
                      },
              }
          },
          methods: {
              rowState(row,rowindex) {
                  return {
                          backgroundColor: '#f4f4f5',
                      }
              },
              close:function() {
                  this.$emit('get-show', false)
              },
              cancel:function(){
                  this.dialogFormVisible = false
              },
              getshow:function(val){
                  this.isShow = val.show;
              },
              save:function(){
                  var i = 0;
                  var _self = this;

                  _self.check_list.length = 0;

                  this.stulist.forEach((element, index) => {
                    if(element.x_status == true){
                      _self.check_list[i++] = element
                    }
                  });

                  if(_self.check_list.length == 0){
                    _self.$message.error('未勾選諮詢學生，請確認!!');
                    return false;
                  }
                  this.$emit('update:dialog_show',true);
                  this.dialogFormVisible = false
                  this.$emit('get-data',_self.check_list);
              },
              query:function(){
                let _self = this
                _self.form.year_id = this.parameter.year_id
                _self.form.sms_id = this.parameter.sms_id
                _self.form.sch_no = this.parameter.sch_no
                _self.form.emp_id = this.parameter.emp_id  
                _self.form.token = this.$token               
                const apiurl = _self.api_interface.get_studata

                if(_self.form.std_name == ""){
                  _self.form.std_name_q = '%'
                }else{
                  _self.form.std_name_q = '%'+_self.form.std_name+'%'
                }
    
                if(_self.form.cls_id == ""){
                  _self.form.cls_id_q = '%'
                }else{
                  _self.form.cls_id_q = _self.form.cls_id
                }

                if(_self.form.std_name != ''){
                  _self.form.sRowNun = 1
                  _self.form.eRowNun = 10
                  _self.currentPage = 1
                  _self.pageSize = 10
                }
                const loading = _self.$loading({
                        lock: true,
                        text: '資料讀取中，請稍後。',
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                        });

                _self.$http({
                  url: apiurl,
                  method: 'get',
                  params: _self.form,
                  headers:{'SkyGet':_self.$token}
                })
                .then((res) => {
                  if (res.data.status == 'Y') {
                    _self.stulist = res.data.dataset
                    _self.total = res.data.dataset[0].x_cnt
                  } else {
                    _self.stulist = []
                    _self.$message.error('查無資料!!')
                  }
                })
                .catch((error) => {
                    _self.stulist = []
                    _self.$message({
                      message: '系統發生錯誤'+error,
                      type: 'error',
                      duration:0,
                      showClose: true,
                    })
                  })
                .finally(() =>{
                  _self.form.std_name = ''
                  loading.close()
                })
              },
              headerclick(column, event){
                      if(column.label == '全選'){
                        column.label = '全不選'
                        this.stulist.forEach((element, index) => {
                          if(element.x_status == false || element.x_status == ''){
                            element.x_status = true
                          }
                        });
                      }else{
                        column.label = '全選'
                        this.stulist.forEach((element, index) => {
                          if(element.x_status == true || element.x_status == ''){
                            element.x_status = false
                          }
                        });
                      }
              },
              current_change(val) {
                      var _self = this;
                      var start = ''
                      var end = ''

                      start = ((val - 1) * _self.pageSize) + 1;
                      end = val * _self.pageSize
                      _self.currentPage = val
                      
                      _self.form.sRowNun = start
                      _self.form.eRowNun = end
                      _self.query()
              },
              size_change(val) {
                var _self = this;
                var start = ''
                var end = ''
                start = ((_self.currentPage - 1) * val) + 1;
                end = _self.currentPage * val
                _self.pageSize = val
                
                _self.form.sRowNun = start
                _self.form.eRowNun = end
                _self.query()
              },
          },
          mounted() {
            let _self = this
            const apiurl = `${_self.$apiroot}/TeaConsult/clsdata`

            _self.parameter.sch_no = this.tea_consult.a
            _self.parameter.year_id = this.tea_consult.b
            _self.parameter.sms_id = this.tea_consult.c
            _self.parameter.emp_id = this.tea_consult.d
            _self.parameter.token = this.$token
            _self.$http({
            url: apiurl,
            method: 'get',
            params: _self.parameter,
            headers:{'SkyGet':_self.$token}
            })
            .then((res) => {
              if (res.data.status == 'Y') {
                _self.clslist = res.data.dataset
              } else {
                _self.clslist = []
              }
            })
            .catch((error) => {
                _self.clslist = []
                _self.$message({
                  message: '系統發生錯誤'+error,
                  type: 'error',
                  duration:0,
                  showClose: true,
                })
              })
            .finally()
          },
          computed: {
                render_header() {
                  let headers = this.data_structure.sub_header.filter((a) => { return a.hidden === 'N' })
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
          }
      }
</script>

<style scoped>
.el-table__expand-icon{
      -webkit-transform: rotate(0deg);
      transform: rotate(0deg);
  }
.el-table__expand-icon .el-icon-arrow-right:before{
      content: "\e7c3";
      font-size:18px;
      color:#303133;
      position:absolute;
      top:-4px;
      /*
      display:table-cell;
      vertical-align:middle;
      border: 1px solid #ccc;
      */
  }
.el-tag{
  color:#555555 !important
}
</style>
