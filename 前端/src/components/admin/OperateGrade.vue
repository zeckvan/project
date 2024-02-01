<template>
    <div>
      <el-table 
      :data="tableData" 
      height=72vh  
      stripe
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
                  <div v-else-if="item.type == 'select'">
                    <el-select disabled style="width:350px" v-model="scope.row[item.prop]" placeholder="">
                        <el-option v-for="item in typeList" :key="item.id" :label="item.name" :value="item.id" >
                        </el-option>        
                    </el-select>  
                  </div>
                  <div v-else-if="item.type == 'date'">
                    <el-date-picker 
                        style="width:350px"
                        :clearable="false"
                        :editable="false"
                        v-model="scope.row[item.prop]" 
                        type="datetime"
                        placeholder="選擇日期時間"                     
                        value-format="yyyy-MM-dd HH:mm:ss">
                    </el-date-picker>                                                     
                  </div>
                </div>
                <div v-else style="text-align: center;">
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
        }
      },
      data() {
        return {
          data_structure: {},
          //tableData:[],
          total: 0,
          currentPage: 1,
          pageSize: 10,
          show_page:true,
          getempid:'',
          getAddStatus:new Date(),    
          typeList:[
            {id:'01',name:' 學生上傳及送出學習成果認證時間'},
            {id:'02',name:'老師認證學生學習成果時間'},
            {id:'03',name:'學生勾選學習成果時間'},
            {id:'04',name:'學生勾選多元表現時間'},
          ]           
        }
      },
      methods: {
        save:function(){
          let _self = this
          let sdate
          let edate
          let formdata = new FormData();
          _self.tableData.forEach(function(value, index, array){ 
            formdata.append('year_id[]',Number(value.year_id))
            formdata.append('sms_id[]',Number(value.sms_id))
            formdata.append('grade_id[]',value.grade_id)
            formdata.append('type_id[]',value.type_id) 
            formdata.append('startdate[]',value.startdate.replaceAll('-','').replaceAll(':','').replaceAll(' ','')) 
            formdata.append('enddate[]',value.enddate.replaceAll('-','').replaceAll(':','').replaceAll(' ','')) 
          });
 
          formdata.append('token',_self.$token)
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          apiurl = _self.api_interface.save_data

          _self.$http({
            url: apiurl,
            method:"put",
            data:formdata,
            headers:{'SkyGet':_self.$token}
          })
            .then((res) => {  
              if (res.data.status == 'Y'){
                  _self.$message.success('存檔成功!!')
              }else{
                  _self.$message.error(res.data.message)
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
        },
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
          headers:{'SkyGet':_self.$token}
        })
          .then((res) => {  
            if (res.data.status == 'Y') {   
              _self.tableData = res.data.dataset   
            } else {
              _self.studata = []
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
      },
      components: {

      },
      beforeDestroy(){
      },
      mounted() {
    
        //this.get_data()
      },
      beforeMount() {
        switch (this.userStatic.data_structure) {
          case 'OperateSetUp':
            this.data_structure = data_structure.Operate_SetUp
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
    .el-input--small .el-input__inner {
        height: 32px;
        line-height: 32px;
        
    }
  </style>
  