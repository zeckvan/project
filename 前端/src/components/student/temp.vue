<template>
  <div>
    <v-pubquery v-on:get-condition="getcondition"></v-pubquery>
    <div align="left">
      <el-button type="primary" v-on:click="add">新增</el-button>
    </div>
    <el-table :data="tableData"
              stripe
              style="width: 100%"
              :row-style="rowState">
              <el-table-column type="index" width="50"></el-table-column>
              <el-table-column v-for="item in render_header"
                  :key = "item.prop"
                  :prop="item.prop"
                  :label="item.label"
                  :width="item.width">
              </el-table-column>
              <el-table-column
                  align="center"
                  fixed="right"
                  label="檢視資料"
                  width="">
                      <template slot-scope="scope">
                        <i class="el-icon-search" style="cursor:pointer" @click="edit_data(scope)"></i>
                      </template>
                </el-table-column>
                <el-table-column
                  align="center"
                  fixed="right"
                  label="上傳檔案"
                  width="">
                      <template slot-scope="scope">
                        <i class="el-icon-upload" style="cursor:pointer" @click="file_upload(scope)"></i>
                      </template>
                </el-table-column>
                <el-table-column
                  align="center"
                  fixed="right"
                  label="查看檔案"
                  width="">
                      <template slot-scope="scope">
                        <i class="el-icon-s-order" style="cursor:pointer" @click="file_data(scope)"></i>
                      </template>
                </el-table-column>
                <el-table-column
                style=""
                  align="center"
                  fixed="right"
                  label="刪除資料"
                  width="">
                      <template slot-scope="scope">
                        <i class="el-icon-delete" style="cursor:pointer" @click="del_data(scope)"></i>
                      </template>
                </el-table-column>

    </el-table>
    <el-pagination
        :page-size="pageSize"
        :total="total"
        layout="total,prev, pager, next,sizes"
        v-on:current-change="current_change"
        v-on:size-change="size_change">
    </el-pagination>
    <component :is='content'
               :dialog_show.sync="dialog_show"
               :edit_model="edit_model"
               :parameter="parameter"
               :data_structure="data_structure"
               :userStatic="userStatic"
               v-on:get-show="getshow"
               v-if="isShow">
    </component>
    <v-stufilelist :dialog_show.sync="dialog_show2"
                    :filelist="filelist"
                    :userStatic="userStatic"
                    :parameter="parameter"
                    v-on:get-show="getshow2" v-if="isShow2">
    </v-stufilelist>
    <v-fileupload :dialog_show.sync="dialog_show3"
                  :filelist="filelist"
                  :userStatic="userStatic"
                  :parameter="parameter"
                  :data_structure="data_structure"
                  :rowdata="rowdata"
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
  import PubQuery from '@/components/pub/pub_query.vue'
  import fileupload from '@/components/pub/pub_upload.vue'

  import * as data_structure from '@/js/stu_grid_structure.js'

  export default {
    props: {
            userStatic: {
              type: Object,
            },
        },
    data() {
      return {
          content: this.userStatic.form_component,
          data_structure:{},
          tableData:[],
          isShow: false,
          isShow2:false,
          isShow3:false,
          dialog_show: true,
          dialog_show2: true,
          dialog_show3: true,
          total:0,
          currentPage: 1,
          pageSize: 10,
          year:'',
          sms:'' ,
          edit_model:'',
          parameter:{},
          filelist:[],
          rowdata:{}
      }
    },
    methods: {
      rowState(row,rowindex) {
        return {
              backgroundColor: '#f4f4f5',
           }
      },
      add:function(){
        this.edit_model = 'add'
        this.isShow = true
      },
      edit_data:function(val){
        let _self = this
        this.data_structure.header.forEach(function(value, index, array){
                              if(value.parameter == "Y"){
                                _self.parameter[value.col]=val.row[value.prop]
                              }
                          });
        this.edit_model = 'edit'
        this.isShow = true
      },
      file_data:function(val){
        let _self = this
        switch (this.userStatic.data_structure) {
          case 'stucadre':
              apiurl = `${this.$apiroot}/${this.userStatic.interface}/files/010106`
              break;
          case 'stustudyf':
              apiurl = `${this.$apiroot}/absenceRecord/010106`
              break;
          case 'stugroup':
              apiurl = `${this.$apiroot}/absenceRecord/010106`
              break;
          default:
        }

        const loading = _self.$loading({
                              lock: true,
                              text: '資料讀取中，請稍後。',
                              spinner: 'el-icon-loading',
                              background: 'rgba(0, 0, 0, 0.7)'
                              });

        _self.data_structure.header.forEach(function(value, index, array){
            if(value.parameter == "Y" && value.col != "std_no"){
              _self.parameter[value.col]=val.row[value.prop]
            }
        });


        _self.$http({
            url:apiurl,
            method:'get',
            params:_self.parameter
            })
            .then((res)=>{
                            if (res.data.status == 'Y'){
                              _self.filelist = res.data.dataset
                            }else{
                                _self.filelist = []
                                _self.$message.error('查無資料，請確認')
                            }
                    })
            .catch((error)=>{
                                _self.tableData = []
                                _self.$message.error('系統發生錯誤:'+error)
                    })
            .finally(()=>loading.close())

        this.isShow2 = true
      },
      file_upload:function(val){
        let _self = this
        switch (this.userStatic.data_structure) {
          case 'stucadre':
              apiurl = `${this.$apiroot}/${this.userStatic.interface}/files`
              break;
          case 'stustudyf':
              apiurl = `${this.$apiroot}/absenceRecord/010106`
              break;
          case 'stugroup':
              apiurl = `${this.$apiroot}/absenceRecord/010106`
              break;
          default:
        }
        _self.rowdata = val
        _self.data_structure.header.forEach(function(value, index, array){
            if(value.parameter == "Y"){
              _self.parameter[value.col]=val.row[value.prop]
            }
        });
        this.isShow3 = true
      },
      del_data:function(val){
        let _self = this
        if(this.data_structure.delete_rule.rule_flag === "Y"){
           if(!this.data_structure.delete_rule.rule_check(val,_self)){
             return false
           }
        }
        _self.$confirm(`確定刪除?`, 'Warning', {
                      confirmButtonText: '確定',
                      cancelButtonText: '取消',
                      type: 'warning'
                  }).then(() => {
                    let formdata = new FormData();

                    _self.data_structure.header.forEach(function(value, index, array){
                              if(value.parameter == "Y" && value.col != "std_no"){
                                _self.parameter[value.col]=val.row[value.prop]
                                formdata.append(value.col,val.row[value.prop]);
                              }
                          });

                    const apiurl = `${this.$apiroot}/${this.userStatic.interface}/010106`
                    _self.$http({
                      headers: { 'Content-Type': 'application/json;charset=utf-8'},
                      url:apiurl,
                      method:'delete',
                      data:formdata
                      })
                      .then((res)=>{
                                    if (res.data.status == 'Y'){
                                        _self.$message.success('刪除成功!!')
                                        _self.tableData.splice(val.$index, 1);
                                    }else{
                                        _self.$message.error('刪除失敗!!')
                                    }
                              })
                      .catch((error)=>{
                                    _self.$message.error('系統發生錯誤:'+error)
                              })
                      .finally(()=>loading.close())
                  }).catch(() => {
                  })
      },
      getshow:function(val){
        this.isShow = val.show;
      },
      getshow2:function(val){
        this.isShow2 = val.show;
      },
      getshow3:function(val){
        this.isShow3 = val.show;
      },
      getcondition:function(val){
        var _self = this;
        _self.year = val.year
        _self.sms = val.sms

        switch (this.userStatic.data_structure) {
          case 'stucadre':
              apiurl = `${this.$apiroot}/${this.userStatic.interface}/list/010106`
              break;
          case 'stustudyf':
              apiurl = `${this.$apiroot}/absenceRecord/010106`
              break;
          default:
        }
        this.callapi(apiurl,val.year,val.sms,_self.currentPage,_self.pageSize)
      },
      current_change(val) {
            var _self = this;
            var start = ''
            var end = ''

            start = ((val - 1) * _self.pageSize) + 1;
            end = val * _self.pageSize
            _self.currentPage = val
            //callapi(apiurl,_self.year,_self.sms,start,end)
          },
      size_change(val) {
            var _self = this;
            var start = ''
            var end = ''

            start = ((_self.currentPage - 1) * val) + 1;
            end = _self.currentPage * val
            _self.pageSize = val
            //callapi(apiurl,_self.year,_self.sms,start,end)
        },
      callapi:function(apiurl,year,sms,start,end){
          let _self = this
          const loading = _self.$loading({
              lock: true,
              text: '資料讀取中，請稍後。',
              spinner: 'el-icon-loading',
              background: 'rgba(0, 0, 0, 0.7)'
            });


          _self.$http({
              url:apiurl,
              method:'get',
              /*
              params:{
                stdno:"010106"
                //year:year,
                //sms:sms
                //start:start,
                //end :end
              }*/
              })
              .then((res)=>{
                              if (res.data.status == 'Y'){
                                _self.tableData = res.data.dataset
                              }else{
                                  _self.tableData = []
                                  _self.$message.error('查無資料，請確認')
                              }
                      })
              .catch((error)=>{
                                  _self.tableData = []
                                  _self.$message.error('系統發生錯誤:'+error)
                      })
              .finally(()=>loading.close())
        }
    },
    components: {
        'v-stucadreform':stucdreform,
        'v-stustudyform':stustudyform,
        'v-pubquery': PubQuery,
        'v-stufilelist':stufilelist,
        'v-fileupload':fileupload,
        'v-stugroupform':stugroupform
    },
    mounted() {

    },
    beforeMount(){
      switch (this.userStatic.data_structure) {
          case 'stucadre':
              this.data_structure  = data_structure.stu_cadre
              break;
          case 'stustudyf':
              this.data_structure  = data_structure.stu_study_f
              break;
          case 'stugroup':
              this.data_structure  = data_structure.stu_group
              break;
          default:
      }
    },
    computed: {
      render_header(){
          let headers = this.data_structure.header.filter((a) =>{return a.hidden === 'N'})
                            .sort(function(a, b) {
                                        if(a.sort > b.sort){
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

<style>
/*
.el-table tbody tr:hover>td{
          background-color:#c0c4cc73 !important
      }
*/
</style>
