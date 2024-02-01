<template>
  <div>
    <el-tag type="info" size="medium" style="font-size: 30px;">{{tag}}</el-tag>
    <div align="left">
      <el-button type="primary" v-on:click="add">新增諮詢學生</el-button>
      <el-button type="danger" @click="del_batch">批次刪除諮詢學生</el-button>
    </div>
    <el-table :data="tableData" stripe style="width: 100%"
      :row-style="rowState"
      v-on:header-click="headerclick"
      v-on:cell-mouse-enter="cellmouseenter" height="60vh">
      <el-table-column type="index" width="50"></el-table-column>
      <el-table-column prop="x_status"
              width="80"
              header-align="center"
              align="center"
              label="全選"
              style="cursor:pointer"
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
      <el-table-column style="" align="center" fixed="right" label="刪除資料" width="">
        <template slot-scope="scope">
          <i class="el-icon-delete" style="cursor:pointer" @click="del_single(scope)"></i>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
      v-on:current-change="current_change" v-on:size-change="size_change">
    </el-pagination>
    <PubStuList :dialog_show.sync="dialog_show"
                :userStatic="userStatic"
                :api_interface="api_interface"
                :data_structure="data_structure"
                v-on:get-show="getshow"
                v-on:get-data="getstudata"
                :parameter="parameter"
                :tea_consult="tea_consult"
                v-if="isShow" />
  </div>
</template>

<script type="module">
    const apiurl = ''
    import * as data_structure from '@/js/tea_grid_structure.js'
    import PubStuList from '@/components/teacher/tea_consult_studata.vue'
    export default {
            props: {
              userStatic: {
                type: Object,
              },
              api_interface: {
                type: Object,
              },
              parentData:{
                type:Array
              },
              total:{
                type:Number
              },
              complex_key:{
                type:Object
              },
              tea_consult:{
                type:Object
              }
            },
            data() {
                return {
                          studata:{
                            sch_no:"",
                            year_id:0,
                            sms_id:0,
                            emp_id:"",
                            ser_id:0,
                            std_no:[]
                          },
                          is_readonly :false,
                          method_type:"post",
                          check_list:[],
                          currentPage: 1,
                          pageSize: 10,
                          data_structure: {},
                          dialog_show:true,
                          isShow:false,
                          parameter:{},
                          tableData:[],
                          stdarray:[],
                          tag:''
                        }
            },
            methods: {
                    getshow: function (val) {
                      this.isShow = val.show;
                    },
                    getstudata:function(val){
                      this.tableData = this.tableData.concat(val) 
                      const result = [];
                      const map = new Map();
                      for (const item of this.tableData) {
                          if(!map.has(item.b)){
                              map.set(item.b, true);
                              result.push(item);
                          }
                      }
                      this.tableData = result.sort(function (a, b) {
                                            if (a.b > b.b) {
                                              return 1
                                            } else {
                                              return -1
                                            }
                                        }
                      )
                      this.tableData.forEach(function(item,index,array){
                        item.x_status = false
                      })
                      this.save()
                    },
                    cellmouseenter(row, column, cell, event){
             
                    },    
                    headercellstyle({ column }) {
                      const whitelist = ["全選", "全不選"];
                      if (whitelist.includes(column.label)) {
                        return {
                          backgroundColor: "red",
                          color: "#333",
                          cursor:pointer      
                        };
                      }
                    },                                    
                    rowState(row, rowindex) {
                      return {
                        backgroundColor: '#f4f4f5',
                      }
                    },
                    add:function(){
                      this.isShow = true
                    },
                    del:function(apiurl,formdata,deldata,type){
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

                        _self.$http({
                          headers: { 'Content-Type': 'application/json;charset=utf-8' ,'SkyGet':_self.$token},
                          url: apiurl,
                          method: 'delete',
                          data: formdata
                        })
                          .then((res) => {
                            if (res.data.status == 'Y') {
                              _self.$message.success('刪除成功!!')
                              if(type == 'del_batch'){
                                for(var i = 0;i < deldata.length;i++){
                                  _self.tableData.forEach(function(item,index,array){
                                      if(deldata[i].b == item.b){                      
                                        _self.tableData.splice(index,1)
                                      }
                                  })                                
                                }
                              }else{
                                if(deldata == 0){
                                  _self.tableData.splice(0, 1);
                                }else{
                                  _self.tableData.splice(deldata, 1);
                                }
                                
                              }

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
                    del_batch:function(){
                        var i = 0;
                        var _self = this;
                        let formdata = new FormData();

                        formdata.append('sch_no', this.tea_consult.a);
                        formdata.append('year_id',this.tea_consult.b);
                        formdata.append('sms_id',this.tea_consult.c);
                        formdata.append('emp_id',this.tea_consult.d);
                        formdata.append('ser_id',this.tea_consult.e);                         
                        formdata.append('token',this.$token);    
                        _self.check_list.length = 0;

                        this.tableData.forEach((element, index) => {
                          if(element.x_status == true){
                            element.row = index
                            _self.check_list[i++] = element
                            formdata.append('std_no[]',element.b);
                          }
                        });
                        
                        if(_self.check_list.length == 0){
                          _self.$message.error('未勾選諮詢學生，請確認!!');
                          return false;
                        }                    
                        const apiurl = _self.api_interface.del_studata
                        _self.del(apiurl,formdata,_self.check_list,'del_batch')
                    },
                    del_single:function(val){
                      let _self = this                      
                      let formdata = new FormData();
                      let element = {"row":val.$index}
                      _self.check_list.length = 0;
                      
                      _self.check_list[0] = element                      
                      formdata.append('std_no[]',val.row.b);
                      formdata.append('sch_no', this.tea_consult.a);
                      formdata.append('year_id',this.tea_consult.b);
                      formdata.append('sms_id',this.tea_consult.c);
                      formdata.append('emp_id',this.tea_consult.d);
                      formdata.append('ser_id',this.tea_consult.e);                      
                      formdata.append('token',this.$token);   

                      const apiurl = _self.api_interface.del_studata
                       _self.del(apiurl,formdata,val.$index,'del_single')

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
                    save:function(){
                        let _self = this
                        let formdata = new FormData();
                        this.tableData.forEach(function(value, index, array){
                                formdata.append('std_no[]',value.b );
                            });
                        
                        formdata.append('sch_no', this.tea_consult.a);
                        formdata.append('year_id',this.tea_consult.b);
                        formdata.append('sms_id',this.tea_consult.c);
                        formdata.append('emp_id',this.tea_consult.d);
                        formdata.append('ser_id',this.tea_consult.e);
                        formdata.append('token',this.$token);

                        const loading = _self.$loading({
                                lock: true,
                                text: '資料處理中，請稍後。',
                                spinner: 'el-icon-loading',
                                background: 'rgba(0, 0, 0, 0.7)'
                                });
                                
                        const apiurl = _self.api_interface.save_stuconsult
                        _self.$http({
                            //'Content-Type': 'multipart/form-data',
                            url:apiurl,
                            method:_self.method_type,
                            data:formdata,
                            headers:{'SkyGet':_self.$token}
                            })
                            .then((res)=>{
                                            if (res.data.status == 'Y'){
                                                _self.$message.success('新增成功!!')
                                            }else{
                                                _self.$message.error(res.data.message)
                                            }
                                    })
                            .catch((error) => {
                                _self.$message({
                                message: '新增失敗:'+error,
                                type: 'error',
                                duration:0,
                                showClose: true,
                                })
                            })
                            .finally(()=>loading.close())

                    },
                    close:function() {

                    },
                    getyear:function(val){
                        this.form.year_id = val
                    },
                    getsms:function(val){
                         this.form.sms_id = val
                    },
                    current_change(val) {
                        var _self = this;
                        var start = ''
                        var end = ''

                        start = ((val - 1) * _self.pageSize) + 1;
                        end = val * _self.pageSize
                        _self.currentPage = val

                      this.get_data(_self.complex_key.a,
                                    _self.complex_key.b,
                                    _self.complex_key.c,
                                    _self.complex_key.d,
                                    _self.complex_key.e,
                                    start, end)
                    },
                    size_change(val) {
                      var _self = this;
                      var start = ''
                      var end = ''
                      start = ((_self.currentPage - 1) * val) + 1;
                      end = _self.currentPage * val
                      _self.pageSize = val
                      this.get_data(_self.complex_key.a,
                                    _self.complex_key.b,
                                    _self.complex_key.c,
                                    _self.complex_key.d,
                                    _self.complex_key.e,
                                    start, end)
                    },
                    get_data: function (sch_no,year, sms,emp_id,ser_id, start, end) {                      
                      let _self = this
                      let apiurl = _self.api_interface.get_stuconsult
                      const loading = _self.$loading({
                        lock: true,
                        text: '資料讀取中，請稍後。',
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                      });

                      _self.parameter.emp_id = emp_id
                      _self.parameter.year_id = year
                      _self.parameter.sms_id = sms
                      _self.parameter.sRowNun = start
                      _self.parameter.eRowNun = end
                      _self.parameter.sch_no = sch_no
                      _self.parameter.ser_id = ser_id

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
                            _self.total = 0
                            _self.$message.error('查無資料!!');
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
                        .finally(()=>loading.close())
                    },
            },
            mounted() {

            },
            components: {
              PubStuList
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
          },
          beforeMount() {
            switch (this.userStatic.data_structure) {
              case 'teaconsult'://學生諮詢課程填寫
                this.data_structure = data_structure.tea_consult
                break;
              default:
            }
          },
          watch:{
            parentData:function(val){
              this.tableData = this.parentData          
              //console.log(this.total)
            },
            tea_consult:function(){
              this.tag = `諮詢日期:${this.tea_consult.f}
                          諮詢地點:${this.tea_consult.g}
                          諮詢方式:${this.tea_consult.h}
                          諮詢主題:${this.tea_consult.i}
                         `
            }
          }
    }
</script>

<style>

</style>
