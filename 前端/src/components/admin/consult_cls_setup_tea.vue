<template>
  <div>
    <el-card>
      教師列表
      <div>
        <el-table :data="teadata" stripe style="width: 100%;" height="77vh">
          <el-table-column type="index" width="40"></el-table-column>
          <el-table-column prop="x_status"
              width="40"
              header-align="center"
              align="center"
              label="勾選"
              >
          <template slot-scope="scope">
              <el-checkbox v-model="scope.row.x_status" @click.native="selectdata(scope)"></el-checkbox>
          </template>
        </el-table-column>          
          <el-table-column
            prop="emp_id"
            label="教師員編"
            width="135"
          >
            <template slot="header" slot-scope="scope">
              <el-input
                v-model="filter_emp_id"
                @input="fiter_empid"
                size="medium"
                placeholder="篩選教師員編"/>
            </template>        
          </el-table-column>
          <el-table-column
            prop="emp_name"
            label="教師姓名"
            width="140"
          >
            <template slot="header" slot-scope="scope">
              <el-input
                v-model="filter_emp_name"
                @input="fiter_empname"              
                size="medium"
                placeholder="篩選教師姓名"/>
            </template>        
          </el-table-column>
          <!--
          <el-table-column
            prop="emp_email"
            label="教郵e-mail"
            width="200"            
          >      
          </el-table-column>  
          -->                     
        </el-table>
        <el-pagination v-if="show_page"
            small
            :page-size="pageSize"
            :total="total" 
            layout="total,prev, pager, next"
            v-on:current-change="current_change_tea" 
            v-on:size-change="size_change_tea"/>     
      </div>
    </el-card>  
  </div>
</template>

<script type="module">
  import * as adminAPI from '@/apis/adminApi.js'

  var apiurl = ''
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
        teadataTemp:[],
        teadata:[],
        total: 0,
        currentPage: 1,
        pageSize: 10,
        parameter_tea:{sRowNun:1,eRowNun:9999,token:this.$token},
        filter_emp_id:'',
        filter_emp_name:'',
        show_page:true
      }
    },
    methods: {
        selectdata:function(val){
          this.teadata.forEach(function(item, index, array){
              item.x_status = false
          });  
          this.$emit('get-empdata',val)      
        },
        fiter_empid:function(){
          let _self = this
          _self.teadata = _self.teadataTemp
          if(_self.filter_emp_id == '' && _self.filter_emp_name == ''){       
            _self.teadata = _self.teadata.filter(function(item, index, array){
                                                return  item.rowNum >= 1 && item.rowNum <= 10
                                              });  
            _self.parameter_tea.sRowNun = 1
            _self.parameter_tea.eRowNun = 10  
            _self.show_page = true                
          }else{
            _self.teadata = _self.teadata.filter(function(item, index, array){
              return  item.emp_name.includes(_self.filter_emp_name) && item.emp_id.includes(_self.filter_emp_id)
            });      
            _self.show_page = false                                                                                      
          }                                                                                          
        },
        fiter_empname:function(){
          let _self = this
          _self.teadata = _self.teadataTemp
          if(_self.filter_emp_id == '' && _self.filter_emp_name == ''){           
            _self.teadata = _self.teadata.filter(function(item, index, array){
                return  item.rowNum >= 1 && item.rowNum <= 10
            });  
            _self.parameter_tea.sRowNun = 1
            _self.parameter_tea.eRowNun = 10  
            _self.show_page = true                
          }else{
            _self.teadata = _self.teadata.filter(function(item, index, array){
              return  item.emp_name.includes(_self.filter_emp_name) && item.emp_id.includes(_self.filter_emp_id)
            });      
            _self.show_page = false                                                                                      
          }                                                                                          
        },        
        current_change_tea(val) {
          var _self = this;
          var start = ''
          var end = ''

          start = ((val - 1) * _self.pageSize) + 1;
          end = val * _self.pageSize
          _self.currentPage = val
          _self.parameter_tea.sRowNun = start
          _self.parameter_tea.eRowNun = end
          _self.teadata = _self.teadataTemp
          _self.teadata = _self.teadata.filter(function(item, index, array){
                                                  return  item.rowNum >= _self.parameter_tea.sRowNun && item.rowNum <=_self.parameter_tea.eRowNun
                                                });        
        },      
        size_change_tea(val) {
          var _self = this;
          var start = ''
          var end = ''
          start = ((_self.currentPage - 1) * val) + 1;
          end = _self.currentPage * val
          _self.pageSize = val
          _self.parameter_tea.sRowNun = start
          _self.parameter_tea.eRowNun = end
          _self.teadata = _self.teadataTemp
          _self.teadata = _self.teadata.filter(function(item, index, array){
                                                  return  item.rowNum >= _self.parameter_tea.sRowNun && item.rowNum <=_self.parameter_tea.eRowNun 
                                                });
        },            
        getEmploee:async function(){
          try {
              let _self = this

              const { data, statusText } = await adminAPI.S90_employee.Get(_self.parameter_tea) 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }

              _self.teadataTemp = data.dataset
              _self.teadata.forEach(function(item,index,array)
              { 
                    item.x_status = false                     
              })                      
              _self.teadata = data.dataset.filter(function(item, index, array)
              {
                return  item.rowNum >= 1 && item.rowNum <= 10
              });                        
              _self.total = data.dataset[0].x_total
          } catch (error) {
            
          } 
        },      
    },
    components: {
    },
    beforeDestroy(){
    },
    mounted() {
     
    },
    beforeMount() {
      this.getEmploee()
    },
    computed: {
    }
  }
</script>

<style scoped>

</style>
