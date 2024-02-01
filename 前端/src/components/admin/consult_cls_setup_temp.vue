<template>
  <div>
    <el-container style="height:auto; border: 1px solid #eee">
        <el-aside width="45%" style="border: 1px solid #eee">
          <el-card>
            <div slot="header" style="padding:2px;">
              <span>教師列單</span>
            </div>
            <div>
              <el-table :data="teadata" stripe style="width: 100%;">
                <el-table-column type="index" width="50"></el-table-column>
                <el-table-column
                  prop="emp_id"
                  label="教師員編"
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
                >
                  <template slot="header" slot-scope="scope">
                    <el-input
                      v-model="filter_emp_name"
                      @input="fiter_empname"              
                      size="medium"
                      placeholder="篩選教師姓名"/>
                  </template>        
                </el-table-column>
                <el-table-column
                  prop="emp_email"
                  label="教郵e-mail"
                >      
                </el-table-column>                
              </el-table>
              <el-pagination v-if="show_page"
                  :page-size="pageSize"
                  :total="total" 
                  layout="total,prev, pager, next,sizes"
                  v-on:current-change="current_change_tea" 
                  v-on:size-change="size_change_tea">
              </el-pagination>
            </div>
          </el-card>  
      </el-aside>
      <el-container style="border: 1px solid #eee">
          <el-header style="text-align: right; font-size: 12px">     
            <el-table :data="clsdata" stripe style="width: 100%">
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
              <el-table-column v-for="item in render_header2"
                :key="item.prop"
                :prop="item.prop"
                :label="item.label"
                :width="item.width"
              >
              </el-table-column>
            </el-table>   
            <el-pagination
                  :page-size="pageSize2"
                  :total="total2" 
                  layout="total,prev, pager, next,sizes"
                  v-on:current-change="current_change_clslist" 
                  v-on:size-change="size_change_clslist">
            </el-pagination>                    
          </el-header> 
          <el-main>
          </el-main>
      </el-container>
  </el-container>
      <!--


      <el-table :data="teadata" stripe style="width: 100%">
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
        <el-table-column v-for="item in render_header3"
          :key="item.prop"
          :prop="item.prop"
          :label="item.label"
          :width="item.width"
        >
        </el-table-column>
      </el-table>
      -->
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
    },
    data() {
      return {
        data_structure: {},
        teadataTemp:[],
        teadata:[],
        clsdata:[],
        consult:[],
        total: 0,
        total2: 0,
        currentPage: 1,
        currentPage2: 1,
        pageSize: 10,
        pageSize2: 10,
        parameter_tea:{sRowNun:1,eRowNun:9999},
        parameter_clslist:{year_id:111,sms_id:1,sRowNun:1,eRowNun:10},
        filter_emp_id:'',
        filter_emp_name:'',
        show_page:true
      }
    },
    methods: {
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
        //this.getEmploee()
      },
      current_change_clslist(val) {
          var _self = this;
          var start = ''
          var end = ''

          start = ((val - 1) * _self.pageSize2) + 1;
          end = val * _self.pageSize2
          _self.currentPage2 = val
          _self.parameter_clslist.sRowNun = start
          _self.parameter_clslist.eRowNun = end
          this.getCls()
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
       // this.getEmploee()
      },     
      size_change_clslist(val) {
        var _self = this;
        var start = ''
        var end = ''
        start = ((_self.currentPage2 - 1) * val) + 1;
        end = _self.currentPage2 * val
        _self.pageSize2 = val
        _self.parameter_tea.sRowNun = start
        _self.parameter_tea.eRowNun = end
        this.getCls()
      },         
      getEmploee:function(){
        let _self = this

        const apiurl = `${_self.$apiroot}/S90_employee`
        _self.$http({
              url:apiurl,
              method:'get',
              params:_self.parameter_tea,
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{
                if (res.data.status == 'Y') {   
                    _self.teadataTemp = res.data.dataset
                    _self.teadata = res.data.dataset.filter(function(item, index, array){
                                                              return  item.rowNum >= 1 && item.rowNum <= 10
                                                            });                        
                    _self.total = res.data.dataset[0].x_total
                  } else {
                    _self.teadata = []
                    _self.$message.error('查無資料，請確認')
                  }                                                           
                })         
              .catch((error)=>{
                        _self.$message.error('呼叫後端【S90_employee】發生錯誤,'+error)
                      })
              .finally()           
      },      
      getCls:function(){
        let _self = this

        const apiurl = `${_self.$apiroot}/S04_stucls_page`
        _self.$http({
              url:apiurl,
              method:'get',
              params:_self.parameter_clslist,
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{
                if (res.data.status == 'Y') {   
                    _self.clsdata = res.data.dataset                      
                    _self.total2 = res.data.dataset[0].x_total
                  } else {
                    _self.clsdata = []
                    _self.$message.error('查無資料，請確認')
                  }                                                           
                })         
              .catch((error)=>{
                        _self.$message.error('呼叫後端【S04_stucls】發生錯誤,'+error)
                      })
              .finally()           
      }
    },
    components: {
    },
    beforeDestroy(){
    },
    mounted() {
      this.getEmploee()
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
      render_header1() {
        let headers = this.data_structure.tea_hedaer.filter((a) => { return a.hidden === 'N' })
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
            /*
      render_header3() {
        let headers = this.data_structure.consult_header.filter((a) => { return a.hidden === 'N' })
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

<style scoped>

</style>
