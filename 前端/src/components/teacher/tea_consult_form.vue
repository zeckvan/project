<template>
    <div>
        <el-dialog title="學生諮詢課程填寫"
            :visible.sync="dialogFormVisible"
            v-on:close="close"
            :fullscreen="false" width="100%"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <el-form ref="ruleForm" :model="form" :rules="rules" label-width="120px">
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="學年：" prop="year_id">
                            <datalistyear v-on:get-year="getyear"  :year_id="form.year_id" placeholder="" style="width:50%;margin-right:100%;" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="學期：" prop="sms_id" placeholder="" style="width:50%;margin-right:100%;">
                            <datalistsms v-on:get-sms="getsms"   :sms_type="sms_type" :sms_id="form.sms_id" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="諮詢日期：" prop="consult_date">
                            <el-date-picker :clearable="true" v-model="form.consult_date" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                :readonly=" is_readonly">
                            </el-date-picker>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="諮詢地點：" prop="consult_area">
                            <el-input v-model="form.consult_area" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                  <el-col :span="8">
                        <el-form-item label="諮詢方式：" prop="consult_type">
                          <el-select v-model="form.consult_type" placeholder="" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false">
                                <el-option v-for="item in consultid" :key="item.id" :label="item.name" :value="item.id" :disabled="form.is_sys === '1'? true:false">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                  <el-col :span="8">
                        <el-form-item label="諮詢主題：" prop="consult_subject">
                            <el-input v-model="form.consult_subject" placeholder="" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                  <el-form-item label="諮詢內容：" prop="consult_content">
                    <el-input v-model="form.consult_content" type="textarea" rows = 5></el-input>
                  </el-form-item>
                </el-row>
            </el-form>
            <!--
              <div align="left">
                <el-button type="primary" @click="add">新增諮詢對象</el-button>
              </div>
              <el-table :data="tableData" stripe style="width: 100%" :row-style="rowState" v-on:header-click="headerclick">
                <el-table-column type="index" width="50"></el-table-column>
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
                <el-table-column style="" align="center" fixed="right" label="刪除資料" width="">
                  <template slot-scope="scope">
                    <i class="el-icon-delete" style="cursor:pointer" @click="del_data(scope)"></i>
                  </template>
                </el-table-column>
              </el-table>

              <el-pagination  :page-size="pageSize"
                              :total="total"
                              layout="total,prev, pager, next,sizes"
                              v-on:current-change="current_change"
                              v-on:size-change="size_change">
              </el-pagination>
            -->

            <el-button type="primary" @click="save">存檔</el-button>
            <el-button type="primary" @click="cancel">取消</el-button>
<!--
            <v-stulist  :dialog_show.sync="dialog_show2"
                        v-on:get-show="getshow2" v-if="isShow2">
            </v-stulist>
-->

        </el-dialog>
    </div>
</template>

<script type="module">
    import datalistyear from '@/components/pub/DataList_year.vue'
    import datalistsms from '@/components/pub/DataList_sms.vue'
    /*
    import stulist from '@/components/teacher/tea_consult_stulist.vue'
*/
    export default {
            props: {
                dialog_show: {
                    type: Boolean
                },
                edit_model:{
                    tyep:String
                },
                data_structure:{
                    type:Object
                },
                parameter:{
                    type:Object
                },
                userStatic: {
                    type: Object,
                },
                api_interface:{
                    type: Object,
                }
            },
            data() {
                return {
                            sms_type: '1',
                            form: {
                                    year_id:'',
                                    sms_id:'',
                                    emp_id:'',
                                    consult_date:'',
                                    consult_area:'',
                                    consult_type:'',
                                    consult_subject:'',
                                    consult_content:'',
                                    is_sys:'',
                                    token:''
                                   },
                            studata:[],
                            rules: {
                                year_id: [
                                        { required: true, message: '請輸入學年', trigger: 'change' }
                                    ],
                                sms_id: [
                                    { required: true, message: '請輸入學期', trigger: 'change' }
                                ],
                                consult_date: [
                                    { required: true, message: '請輸入諮詢日期', trigger: 'change' }
                                ],
                                consult_area: [
                                    { required: true, message: '請輸入諮詢地點', trigger: 'change' }
                                ],
                                consult_type: [
                                    { required: true, message: '請輸入諮詢方式', trigger: 'change' }
                                ],
                                consult_subject: [
                                    { required: true, message: '請輸入諮詢主題', trigger: 'change' }
                                ],
                                consult_content: [
                                    { required: true, message: '請輸入諮詢內容', trigger: 'change' }
                                ],
                            },
                            dialogFormVisible: this.dialog_show,
                            consultid:[{id:"1",name:"團體諮詢"},
                                       {id:"2",name:"個別諮詢"}],
                            is_readonly :false,
                            method_type:"post",
                            dialog_show2: true,
                            isShow2: false,
                            total: 0,
                            currentPage: 1,
                            pageSize: 10,
                }
            },
            methods: {
                    rowState(row, rowindex) {
                      return {
                        backgroundColor: '#f4f4f5',
                      }
                    },
                    add:function(){

                    },
                    del_data:function(){

                    },
                    headerclick(column, event){
                      if(column.label == '全選'){
                        column.label = '全不選'
                        this.studata.forEach((element, index) => {
                          if(element.x_status == false || element.x_status == ''){
                            element.x_status = true
                          }
                        });
                      }else{
                        column.label = '全選'
                        this.studata.forEach((element, index) => {
                          if(element.x_status == true || element.x_status == ''){
                            element.x_status = false
                          }
                        });
                      }
                    },
                    cancel:function(){
                        this.dialogFormVisible = false
                    },
                    save:function(){
                        let _self = this
                        let formdata = new FormData();

                        _self.form.emp_id = ''
          
                        this.data_structure.header.forEach(function(value, index, array){
                                formdata.append(value.col,_self.form[value.col]);
                            });
                        formdata.append('token',_self.$token);
                        _self.$refs["ruleForm"].validate((valid) => {
                            if (valid) {
                                const loading = _self.$loading({
                                        lock: true,
                                        text: '資料讀取中，請稍後。',
                                        spinner: 'el-icon-loading',
                                        background: 'rgba(0, 0, 0, 0.7)'
                                        });

                                const apiurl = _self.api_interface.save_form
                                _self.$http({
                                    //'Content-Type': 'multipart/form-data',
                                    url:apiurl,
                                    method:_self.method_type,
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
                                this.dialogFormVisible = false
                            }
                            else{
                                return false
                            }
                        })
                    },
                    close:function() {
                        this.$emit('get-show', false)
                    },
                    getyear:function(val){
                        this.form.year_id = val
                    },
                    getsms:function(val){
                         this.form.sms_id = val
                    },
                    getshow2: function (val) {
                      this.isShow2 = val.show;
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

                    _self.parameter.std_no = ''
                    _self.parameter.year_id = year
                    _self.parameter.sms_id = sms
                    _self.parameter.sRowNun = start
                    _self.parameter.eRowNun = end
                    _self.parameter.sch_no = ''

                    _self.$http({
                      url: apiurl,
                      method: 'get',
                      params: _self.parameter,
                      headers:{'SkyGet':_self.$token}
                    })
                      .then((res) => {
                        if (res.data.status == 'Y') {
                          _self.tableData = res.data.dataset
                          _self.total = res.data.total
                        } else {
                          _self.tableData = []
                          _self.$message.error('查無資料，請確認')
                        }
                      })
                      .catch((error) => {
                        console.log(error)
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
            },
            mounted() {
                let _self = this

                if(this.edit_model === "add"){
                    this.is_readonly = false
                    this.method_type = "post"
                    this.data_structure.header.forEach(function(value, index, array){
                                                _self.form[value.col] = ''
                                          });
                    this.form.is_sys = "2"
                }else{
                    const loading = _self.$loading({
                                    lock: true,
                                    text: '資料讀取中，請稍後。',
                                    spinner: 'el-icon-loading',
                                    background: 'rgba(0, 0, 0, 0.7)'
                                    });

                    const apiurl = _self.api_interface.get_form
                    _self.$http({
                        url:apiurl,
                        method:'get',
                        params:this.parameter,
                        headers:{'SkyGet':_self.$token}
                        })
                        .then((res)=>{
                                        if (res.data.status == 'Y'){
                                            this.data_structure.header.forEach(function(value, index, array){
                                                if(res.data.dataset[0][value.col]){
                                                    if(value.col == "year_id" || value.col == "sms_id"){
                                                        _self.form[value.col] = res.data.dataset[0][value.col].toString()
                                                    }else if(value.col == "consult_date"){
                                                        _self.form[value.col] = res.data.dataset[0][value.col] == "undefined" ? "":res.data.dataset[0][value.col]
                                                    }else{
                                                        _self.form[value.col] = res.data.dataset[0][value.col]
                                                    }
                                                }
                                            })
                                            _self.is_readonly = (res.data.dataset[0].is_sys === "1"? true : false)
                                            //_self.studata = res.data.subdataset[0]
                                        }else{
                                            _self.$message.error('查無資料，請確認')
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
                        .finally(()=>loading.close())
                        this.method_type = "put"
                    }
            },
            components: {
                datalistyear,
                datalistsms,
                //stulist
            },
            computed: {
              /*
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
                */
          }
    }
</script>

<style>

</style>
