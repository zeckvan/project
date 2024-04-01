<template>
  <el-card>
      班級列表
      <el-tooltip style="margin: 4px;" effect="dark" content="加入課程諮詢教師" placement="top-start">
        <i class="el-icon-circle-plus " style="cursor:pointer;float: right; padding: 3px 0" @click="add_data()"></i>
      </el-tooltip>        
    <div>
      <el-table :data="clsdata" height="77vh" stripe style="width: 100%"  v-on:header-click="headerclick" :header-cell-style="headercellstyle">
        <el-table-column type="index" width="40"></el-table-column>
        <el-table-column prop="x_status"
              width="70"
              header-align="center"
              align="center"
              label="全選"
              >
          <template slot-scope="scope">
              <el-checkbox v-model="scope.row.x_status"></el-checkbox>
          </template>
        </el-table-column>
        <el-table-column v-for="item in render_header2"
          :key="item.prop"
          :prop="item.prop"
          :label="item.label"
          :width="item.width"
        >
        </el-table-column>      
      </el-table>   
      <el-pagination v-if="show_page"
          small
          :page-size="pageSize"
          :total="total" 
          layout="total,prev, pager, next"
          v-on:current-change="current_change" 
          v-on:size-change="size_change"/>      
    </div>
  </el-card>
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
      },
      getempid:{
        type:String
      }
    },
    data() {
      return {
        data_structure: {},
        clsdataTemp:[],
        teadata:[],
        clsdata:[],
        consult:[],
        check_list:[],
        total: 0,
        currentPage: 1,
        pageSize: 10,
        parameter_clslist:{year_id:111,sms_id:1,sRowNun:1,eRowNun:9999,token:this.$token},
        filter_emp_id:'',
        filter_emp_name:'',
        show_page:true
      }
    },
    methods: {
      headercellstyle:function(row, column, rowIndex, columnIndex){
         if(row.columnIndex == 1){
          return {cursor:'pointer'}
         }
      },    
      headerclick(column, event){
          if(column.label == '全選' || column.label == '全不選'){
            if(column.label == '全選'){
            column.label = '全不選'
            this.clsdata.forEach((element, index) => {
              if(element.x_status == false || element.x_status == ''){
                element.x_status = true
              }
            });
            }else{
              column.label = '全選'
              this.clsdata.forEach((element, index) => {
                if(element.x_status == true || element.x_status == ''){
                  element.x_status = false
                }
              });
            }
          }
      },           
      current_change(val) {
          var _self = this;
          var start = ''
          var end = ''

          start = ((val - 1) * _self.pageSize) + 1;
          end = val * _self.pageSize
          _self.currentPage = val
          _self.parameter_clslist.sRowNun = start
          _self.parameter_clslist.eRowNun = end
          _self.clsdata = _self.clsdataTemp
          _self.clsdata = _self.clsdata.filter(function(item, index, array){
                                              return  item.rowNum >= _self.parameter_clslist.sRowNun && item.rowNum <=_self.parameter_clslist.eRowNun 
                                            });
      },          
      size_change(val) {
        var _self = this;
        var start = ''
        var end = ''
        start = ((_self.currentPage - 1) * val) + 1;
        end = _self.currentPage * val
        _self.pageSize = val
        _self.parameter_clslist.sRowNun = start
        _self.parameter_clslist.eRowNun = end
        _self.clsdata = _self.clsdataTemp
        _self.clsdata = _self.clsdata.filter(function(item, index, array){
                                              return  item.rowNum >= _self.parameter_clslist.sRowNun && item.rowNum <=_self.parameter_clslist.eRowNun 
                                            });
      },              
      getCls:async function(){
        try {
              let _self = this

              const { data, statusText } = await adminAPI.S04_stucls_page.Get(_self.parameter_clslist) 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }
  
              _self.clsdataTemp = data.dataset
              _self.clsdata = data.dataset 
              _self.clsdata.forEach(function(item,index,array){
                  item.x_status = false                     
              })    

              _self.clsdata = data.dataset.filter(function(item, index, array){
                  return  item.rowNum >= 1 && item.rowNum <= 10
               });      
                                                      
              _self.total = data.dataset[0].x_total
              
          } catch (error) {
            
          }       
      },
      add_data:async function(){
        try {
              var i = 0;
              var _self = this;
              let formdata = new FormData();

              _self.check_list.length = 0;

              this.clsdata.forEach((element, index) => {
                if(element.x_status == true){
                  _self.check_list[i++] = element
                  formdata.append('year_id[]',element.year_id);
                  formdata.append('sms_id[]',element.sms_id);
                  formdata.append('deg_id[]',element.deg_id);
                  formdata.append('dep_id[]',element.dep_id);
                  formdata.append('bra_id[]',element.bra_id);
                  formdata.append('grd_id[]',element.grd_id);
                  formdata.append('cls_id[]',element.cls_id);
                  formdata.append('emp_id[]',this.getempid);
                }
              });
              formdata.append('token',this.$token);
              if(this.getempid == '' || this.getempid == null){
                _self.$message.error('未勾新增課程教師，請確認!!');
                return false;
              }  

              if(_self.check_list.length == 0){
                _self.$message.error('未勾新增課程班級，請確認!!');
                return false;
              }     

              const confirm =  await _self.$confirm(`確定新增課程諮詢教師?`, 'Warning', {
                confirmButtonText: '確定',
                cancelButtonText: '取消',
                type: 'warning'
              }).catch(()=>{})

              if(confirm !== 'confirm')
              {
                return  _self.$message.info('已取消新增!!')
              }

              const { data, statusText } = await adminAPI.consult_setup.Post(formdata) 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }

              if (data.status == 'Y'){
                  _self.$message.success('新增成功!!')
                  _self.$emit('get-insertStatus','')      
              }else{
                  _self.$message.error(data.message)
              }
          } catch (error) {
            
          }          
      },      
    },
    components: {
    },
    beforeDestroy(){
    },
    mounted() {
      this.getCls()
    },
    beforeMount() {
      switch (this.userStatic.data_structure) {
        case 'consult_cls':
          this.data_structure = data_structure.consult_cls
          break;
        default:
      }
    },
    computed: {
      render_header2() {
        let headers = this.data_structure.cls_header.filter((a) => { return a.hidden === 'N' })
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

<style scoped>

</style>
