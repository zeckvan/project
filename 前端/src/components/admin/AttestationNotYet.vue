<template>
    <div>
      <el-container style="height:100%; border: 1px solid #eee">
        <el-main style="border: 1px solid #eee;width: 5%;">
            <el-table :data="tableData" height=77vh  stripe style="width:100%;" :row-style="rowState" v-on:row-click="rowClick" v-on:row-dblclick="rowdblclick">
                <el-table-column type="index" width="50"></el-table-column>
                <el-table-column v-for="item in render_LeftHeader"
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
        <el-main style="border: 1px solid #eee;width: 14%;">
          <el-table :data="studata" height=77vh stripe style="width: 100%" :row-style="rowState">
                <el-table-column type="index" width="50"></el-table-column>
                <el-table-column v-for="item in render_RightHeader"
                    :key="item.prop"
                    :prop="item.prop"
                    :label="item.label"
                    :width="item.width"
                    align="center"
                >  
                </el-table-column>
            </el-table> 
            <el-pagination :page-size="pageSize2" :total="total2" layout="total,prev, pager, next,sizes"
                v-on:current-change="current_change_std" v-on:size-change="size_change_std">
            </el-pagination>             
        </el-main>         
      </el-container>
    </div>
  </template>
  
  <script type="module">
    var apiurl = ''
    import * as data_structure from '@/js/tea_grid_structure.js'

    
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
          data_structure: {},
          tableData:[],
          studata:[],
          clsdata:[],
          consult:[],
          total: 0,
          total2: 0,
          currentPage: 1,
          currentPage2: 1,
          pageSize: 10,
          pageSize2: 10,
          filter_emp_id:'',
          filter_emp_name:'',
          show_page:true,
          getempid:'',
          getAddStatus:new Date(),
          queryform:{
                      year_id:"111",
                      sms_id:"1",
                      cls_id:'',  
                      std_no:'',
                      std_name:'',
                      emp_id:"",
                      consult_emp:"",
                      kind:this.userStatic.interface,
                      sRowNun:1,
                      eRowNun:10,token:this.$token},          
        }
      },
      methods: {
        rowClick:function(row, column, event){
          this.queryform.emp_id = row.emp_id
          this.get_stddata()
        },
        rowdblclick:function(){

        },
        rowState:function(){

        },
        getempdata:function(val){
          this.getempid = val.row.emp_id
        },
        getinsertStatus:function(val){
          this.getAddStatus = new Date()
        },
        current_change:function(){

        },
        current_change_std:function(){

        },
        size_change:function(){

        },
        size_change_std:function(){

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
        get_stddata:function(){
          let _self = this
          const loading = _self.$loading({
          lock: true,
          text: '資料讀取中，請稍後。',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });
        apiurl = _self.api_interface.get_stddata

        _self.$http({
          url: apiurl,
          method: 'get',
          params: _self.queryform,
          headers:{'SkyGet':_self.$token}
        })
          .then((res) => {  
            if (res.data.status == 'Y') {   
              _self.studata = res.data.dataset   
              _self.total2 = res.data.dataset[0].x_total
            } else {
              _self.studata = []
              _self.$message.error('查無資料，請確認')
            }
          })
          .catch((error) => {
              _self.studata = []
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
      components: {

      },
      beforeDestroy(){
      },
      mounted() {
        this.get_data()
      },
      beforeMount() {
        switch (this.userStatic.data_structure) {
          case 'AttestationNotYet':
            this.data_structure = data_structure.attestation_notyet
            break;
          default:
        }
      },
      computed: {
        render_LeftHeader() {
            let headers = this.data_structure.LeftHeader.filter((a) => { return a.hidden === 'N' })
                .sort(function (a, b) {
                if (a.sort > b.sort) {
                    return 1
                } else {
                    return -1
                }
                }
                )
            return headers
        },
        render_RightHeader() {
            let headers = this.data_structure.RightHeader.filter((a) => { return a.hidden === 'N' })
                .sort(function (a, b) {
                if (a.sort > b.sort) {
                    return 1
                } else {
                    return -1
                }
                }
                )
            return headers
        },     
      }
    }
  </script>
  
  <style>

  </style>
  