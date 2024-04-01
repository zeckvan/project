<template>
    <div>
      <el-table 
      :data="tableData" 
      height=77vh  
      stripe s
      tyle="width:100%;" 
      :row-style="rowState"
      :header-cell-style="headercellstyle"
      :cell-style="cellstyle"
      border>
        <el-table-column type="index" width="50" align="center"></el-table-column>     
        <el-table-column  v-for="item in render_header"
          :key="item.prop"
          :prop="item.prop"
          :label="item.label"
          :width="item.width"
          :align="item.align">
            <template slot-scope="scope" >
                <div v-if="item.slot">                  
                  <div v-if="item.type == 'input'">
                    <el-input v-model="scope.row[item.prop]"></el-input>
                  </div>
                </div>
                <div v-else>
                  {{scope.row[item.prop]}}
                </div>
              </template>         
        </el-table-column>        
      </el-table>
      <br/>
      <el-button type="primary" @click="save">存檔</el-button>
    </div>
  </template>
  
  <script type="module">
    var apiurl = ''
    import * as data_structure from '@/js/pub_grid_structure.js'
    import * as adminAPI from '@/apis/adminApi.js'
    
    export default {
      props: {
        userStatic: {
          type: Object,
        },
        api_interface: {
          type: Object,
        }
      },
      data() {
        return {
          data_structure: {},
          tableData:[],
          total: 0,
          currentPage: 1,
          pageSize: 10,
          show_page:true,
          getempid:'',
          getAddStatus:new Date(),       
        }
      },
      methods: {
        headercellstyle:function(row, column, rowIndex, columnIndex){
          return {
            "text-align": 'center',
            "border-left": ' 3px solid #EBEEF5',
            "border-bottom": ' 3px solid #EBEEF5',
            "border-top": ' 3px solid #EBEEF5',
          }
        },
        cellstyle:function(row, column, rowIndex, columnIndex){
          return {
            "border-left": ' 3px solid #EBEEF5',
            "border-bottom": ' 3px solid #EBEEF5',           
          }
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
        save:async function(){
          try {
              let _self = this
              let formdata = new FormData();

              _self.tableData.forEach(function(value, index, array)
              {
                formdata.append('Name[]',value.name)
                formdata.append('Value[]',value.value)
                formdata.append('Unit[]',value.unit)
                formdata.append('Memo[]',value.memo) 
              });

              formdata.append('token',_self.$token) 

              const { data, statusText } = await adminAPI.SystemSetup.Update(formdata) 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }

              if (data.status == 'Y'){
                _self.$message.success('存檔成功!!')
              }else{
                _self.$message.error(res.data.message)
              }
          } catch (error) {
            
          } 
        },
        get_data:async function(){
          try {
              let _self = this
              const { data, statusText } = await adminAPI.SystemSetup.Get() 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }

              _self.tableData = data.dataset 
          } catch (error) {
            
          }      
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
          case 'SystemSetup':
            this.data_structure = data_structure.System_SetUp
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
      }
    }
  </script>
  
  <style>

  </style>
  @/apis/adminApi.js