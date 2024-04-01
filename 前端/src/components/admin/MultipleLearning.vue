<template>
  <div>
      <el-table :data="tableData" height=70vh stripe style="width: 100%" :row-style="rowState" v-on:row-click="rowClick" v-on:row-dblclick="rowdblclick">
          <el-table-column type="index" width="50"></el-table-column>
          <el-table-column v-for="item in render_header"
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
  </div>
</template>

<script type="module">
var apiurl = ''
import * as data_structure from '@/js/tea_grid_structure.js'
import * as adminAPI from  '@/apis/adminApi.js' 

export default {
  props: {
    userStatic: {
      type: Object,
    },
    api_interface: {
      type: Object,
    },
    activePage:{
      type:String
    },
    tableDataPass:{
      type: Array,
    },
    totalPass:{
      type:Number
    },
    queryform:{
      type: Object,
    }
  },
  data() {
    return {
        tableData:[],
        data_structure: {},
        yearlist:[],
        smslist:[],
        clslist:[],
        clsParms:{year_id:'',sms_id:'',emp_id:'10615'},
        total: 0,
        currentPage: 1,
        pageSize: 10,
    }
  },
  methods: {
    rowdblclick:function(){
      this.$emit('dbclick-row',"")      
    },
    rowClick:function(row, column, event){
      this.$emit('get-std', row.std_no)
    },
    rowState(row, rowindex) {
      return {
        backgroundColor: '#f4f4f5',
      }
    },   
    current_change(val) {
        var _self = this;
        var start = ''
        var end = ''

        start = ((val - 1) * _self.pageSize) + 1;
        end = val * _self.pageSize
        _self.currentPage = val
        _self.queryform.sRowNun = start
        _self.queryform.eRowNun = end
        this.get_data()
    },
    size_change(val) {
        var _self = this;
        var start = ''
        var end = ''
        start = ((_self.currentPage - 1) * val) + 1;
        end = _self.currentPage * val
        _self.pageSize = val
        _self.queryform.sRowNun = start
        _self.queryform.eRowNun = end
        this.get_data()
    },  
    get_data:function(apiurl){
      /**
        let _self = this
            const loading = _self.$loading({
              lock: true,
              text: '資料讀取中，請稍後。',
              spinner: 'el-icon-loading',
              background: 'rgba(0, 0, 0, 0.7)'
            });
            _self.$http({
              url: _self.api_interface.get_data,
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
       */
    },
    query:function(){     
      this.get_data()
    },        
  },
  components: {

  },
  beforeDestroy(){

  },
  mounted() {

  },
  beforeMount() {
    switch (this.userStatic.data_structure) {
      case 'MultipleLearningView'://導師查看學生學習成果及多元表現
        this.data_structure = data_structure.multiple_learning
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
  },
  watch:{
    totalPass:function(val1,val2){
      this.total = val1
    },
    tableDataPass:function(val1,val2){
      this.tableData = val1
    }
  },
  created() {

  }
}

</script>

<style>

</style>