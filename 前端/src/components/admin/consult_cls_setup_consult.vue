<template>
  <div>
    <el-card>
      課程諮詢教師
      <el-tooltip style="margin: 4px;" effect="dark" content="刪除課程諮詢教師" placement="top-start">
        <i class="el-icon-remove" style="cursor:pointer;float: right; padding: 3px 0" @click="del_data()"></i>
      </el-tooltip>           
      <div>
        <el-table :data="consult" height="77vh" stripe style="width: 100%;" v-on:header-click="headerclick" :header-cell-style="headercellstyle">
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
          <el-table-column
            prop="emp_id"
            label="教師員編"
            width="140"
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
          <el-table-column
            prop="cls_abr"
            label="班級"
            width="160"            
          >     
          <template slot="header" slot-scope="scope">
              <el-input
                v-model="filter_cls"
                @input="fiter_clsname"              
                size="medium"
                placeholder="篩選教師班級"/>
            </template>            
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
      getAddStatus:{
        type: Date
      }
    },
    data() {
      return {
        data_structure: {},
        consultTemp:[],
        consult:[],
        check_list:[],
        total: 0,
        currentPage: 1,
        pageSize: 10,
        parameter_clslist:{year_id:111,sms_id:1,sRowNun:1,eRowNun:9999,token:this.$token},
        filter_emp_id:'',
        filter_emp_name:'',
        filter_cls:'',
        show_page:true
      }
    },
    methods: {
      del_data:function(){
        var i = 0;
        var _self = this;
        let formdata = new FormData();

        _self.check_list.length = 0;
        apiurl =  _self.api_interface.deleteConsult_SetUp
        this.consult.forEach((element, index) => {
          if(element.x_status == true){
            _self.check_list[i++] = element
            formdata.append('year_id[]',element.year_id);
            formdata.append('sms_id[]',element.sms_id);
            formdata.append('deg_id[]',element.deg_id);
            formdata.append('dep_id[]',element.dep_id);
            formdata.append('bra_id[]',element.bra_id);
            formdata.append('grd_id[]',element.grd_id);
            formdata.append('cls_id[]',element.cls_id);
            formdata.append('emp_id[]',element.emp_id);
          }
        });
        formdata.append('token',this.$token);
        if(_self.check_list.length == 0){
          _self.$message.error('未勾刪除課程諮詢教師，請確認!!');
          return false;
        }     

        _self.$confirm(`確定刪除課程諮詢教師?`, 'Warning', {
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
            headers: { 'Content-Type': 'application/json;charset=utf-8','SkyGet':_self.$token },
            url: apiurl,
            method: 'delete',
            data: formdata
          })
            .then((res) => {
              if (res.data.status == 'Y') {
                _self.$message.success('刪除成功!!')
                _self.parameter_clslist.sRowNun = 1
                _self.parameter_clslist.eRowNun = 9999   
                _self.currentPage = 1
                _self.pageSize = 10               
                _self.getConsult()
                /*
                for(var i = 0;i < _self.check_list.length;i++){
                  _self.consult.forEach(function(item,index,array){
                      if(_self.check_list[i].deg_id == item.deg_id &&
                        _self.check_list[i].dep_id == item.dep_id &&
                        _self.check_list[i].bra_id == item.bra_id &&
                        _self.check_list[i].grd_id == item.grd_id &&
                        _self.check_list[i].cls_id == item.cls_id &&
                        _self.check_list[i].emp_id == item.emp_id ){                      
                        _self.consult.splice(index,1)
                      }
                  })

                  _self.consultTemp.forEach(function(item,index,array){
                      if(_self.check_list[i].deg_id == item.deg_id &&
                        _self.check_list[i].dep_id == item.dep_id &&
                        _self.check_list[i].bra_id == item.bra_id &&
                        _self.check_list[i].grd_id == item.grd_id &&
                        _self.check_list[i].cls_id == item.cls_id &&
                        _self.check_list[i].emp_id == item.emp_id ){                      
                        _self.consultTemp.splice(index,1)
                      }
                  })                  
                  _self.total = _self.consultTemp.length
                  
                  _self.consult = _self.consultTemp
                  _self.consult.forEach(function(item,index,array){ 
                        item.x_status = false                     
                    })                     
                  _self.consult = _self.consult.filter(function(item, index, array){
                                                            return  item.rowNum >= 1 && item.rowNum <= 10
                                                          });                                      
                }     
                */         
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
      headercellstyle:function(row, column, rowIndex, columnIndex){
         if(row.columnIndex == 1){
          return {cursor:'pointer'}
         }
      },    
      headerclick(column, event){
          if(column.label == '全選' || column.label == '全不選'){
            if(column.label == '全選'){
            column.label = '全不選'
            this.consult.forEach((element, index) => {
              if(element.x_status == false || element.x_status == ''){
                element.x_status = true
              }
            });
            }else{
              column.label = '全選'
              this.consult.forEach((element, index) => {
                if(element.x_status == true || element.x_status == ''){
                  element.x_status = false
                }
              });
            }
          }
        },        
        fiter_empid:function(){
          let _self = this
          _self.consult = _self.consultTemp
          if(_self.filter_emp_id == '' && _self.filter_emp_name == '' && _self.filter_cls == ''){       
            _self.consult = _self.consult.filter(function(item, index, array){
                                                return  item.rowNum >= 1 && item.rowNum <= 10
                                              });  
            _self.parameter_clslist.sRowNun = 1
            _self.parameter_clslist.eRowNun = 10  
            _self.show_page = true                
          }else{
            _self.consult = _self.consult.filter(function(item, index, array){
              return  item.emp_name.includes(_self.filter_emp_name) && item.emp_id.includes(_self.filter_emp_id) && item.cls_abr.includes(_self.filter_cls)
            });    
            _self.show_page = false                                                                                     
          }                                                                                          
        },
        fiter_empname:function(){
          let _self = this
          _self.consult = _self.consultTemp
          if(_self.filter_emp_id == '' && _self.filter_emp_name == '' && _self.filter_cls == ''){           
            _self.consult = _self.consult.filter(function(item, index, array){
                return  item.rowNum >= 1 && item.rowNum <= 10
            });  
            _self.parameter_clslist.sRowNun = 1
            _self.parameter_clslist.eRowNun = 10  
            _self.show_page = true                
          }else{
            _self.consult = _self.consult.filter(function(item, index, array){
              return  item.emp_name.includes(_self.filter_emp_name) && item.emp_id.includes(_self.filter_emp_id) && item.cls_abr.includes(_self.filter_cls)
            });     
            _self.show_page = false                                                                                       
          }                                                                                          
        },      
        fiter_clsname:function(){
          let _self = this
          _self.consult = _self.consultTemp
          if(_self.filter_emp_id == '' && _self.filter_emp_name == '' && _self.filter_cls == ''){           
            _self.consult = _self.consult.filter(function(item, index, array){
                return  item.rowNum >= 1 && item.rowNum <= 10
            });  
            _self.parameter_clslist.sRowNun = 1
            _self.parameter_clslist.eRowNun = 10  
            _self.show_page = true                
          }else{
            _self.consult = _self.consult.filter(function(item, index, array){
              return  item.emp_name.includes(_self.filter_emp_name) && item.emp_id.includes(_self.filter_emp_id) && item.cls_abr.includes(_self.filter_cls)
            });
            _self.show_page = false                                                                                           
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
          _self.consult = _self.consultTemp
          _self.consult = _self.consult.filter(function(item, index, array){
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
        _self.consult = _self.consultTemp
        _self.consult = _self.consult.filter(function(item, index, array){
                                                return  item.rowNum >= _self.parameter_clslist.sRowNun && item.rowNum <=_self.parameter_clslist.eRowNun 
                                              });
      },                 
      getConsult:function(){
        let _self = this

        const apiurl = `${_self.$apiroot}/TeaConsult/Get_consult_setup`
        _self.$http({
              url:apiurl,
              method:'get',
              params:_self.parameter_clslist,
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{
                if (res.data.status == 'Y') {   
                    _self.consultTemp = []
                    _self.consult = []
                    _self.consultTemp = res.data.dataset
                    _self.consult = res.data.dataset 
                    _self.consult.forEach(function(item,index,array){ 
                          item.x_status = false                     
                      })                     
                    _self.consult = res.data.dataset.filter(function(item, index, array){
                                                              return  item.rowNum >= 1 && item.rowNum <= 10
                                                            }); 
                                                                          
                    _self.total = res.data.dataset[0].x_total                         
                  } else {
                    _self.consult = []
                   // _self.$message.error('查無資料，請確認')
                  }                                                           
                })         
              .catch((error)=>{
                        _self.$message.error('呼叫後端【Get_consult_setup】發生錯誤,'+error)
                      })
              .finally()           
      }
    },
    components: {
    },
    beforeDestroy(){
    },
    mounted() {

    },
    beforeMount() {
      this.getConsult()
    },
    computed: {
    },
    watch:{
      getAddStatus:function(){
        this.parameter_clslist.sRowNun = 1
        this.parameter_clslist.eRowNun = 9999   
        this.currentPage = 1
        this.pageSize = 10          
        this.getConsult()
      }
    }
  }
</script>

<style scoped>

</style>
