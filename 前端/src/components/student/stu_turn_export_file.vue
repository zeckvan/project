<template>
  <div>
    <el-table :data="tableData" stripe style="width: 100%"
      :row-style="rowState"
      v-on:header-click="headerclick"
      height="60vh">
      <el-table-column type="index" width="50"></el-table-column>
      <el-table-column v-for="item in render_header"
        :key="item.prop"
        :prop="item.prop"
        :label="item.label"
        :width="item.width"
      >
      </el-table-column>
      <el-table-column label="下載檔案" width="100" align="center">
          <template slot-scope="scope">
                  <i class="el-icon-picture" style="cursor:pointer" @click="file_download(scope)"></i>
          </template>
       </el-table-column>
    </el-table>

    <el-pagination :page-size="pageSize" :total="total" layout="total,prev, pager, next,sizes"
      v-on:current-change="current_change" v-on:size-change="size_change">
    </el-pagination>
  </div>
</template>

<script type="module">
import * as data_structure from '@/js/stu_grid_structure.js'
import PubStuList from '@/components/pub/pub_studata.vue'
var apiurl = ''

export default {
  props: {
    userStatic: {
      type: Object,
    },
    api_interface: {
      type: Object,
    },
    total:{
      type:Number
    },
    tableData:{
      type:Array
    }
  },
  data() {
    return {
      currentPage: 1,
      pageSize: 10,
      data_structure: {},
      dialog_show:true,
      isShow:false,
      //tableData:[],
      check_list:[],
      sch_no:'',
      filename:''
    }
  },
  methods: {
    file_download:function(val)
    {
      var _self = this
      const loading = _self.$loading({
        lock: true,
        text: '資料轉出中，請稍後。',
        spinner: 'el-icon-loading',
        background: 'rgba(0, 0, 0, 0.7)'
        });

        apiurl = _self.api_interface.export_file

        _self.$http({
          headers: { 'SkyGet':_self.$token},
          url: apiurl,
          method: 'get',
          params: {id:val.row.id},
          responseType: 'blob',
        })
        .then((res)=>{
              _self.download(res,val)
         })
        .catch((error)=>{
              _self.$message.error('檔案下載發生錯誤:'+error)
         })
        .finally(()=>loading.close())
    },
    del_single: function (val)
    {
      if(val.$index == 0){
        this.tableData.splice(0, 1);
      }else{
        this.tableData.splice(val.$index, 1);
      }
    },
    rowState(row, rowindex) 
    {
      return {
        backgroundColor: '#f4f4f5',
      }
    },
    headerclick(column, event)
    {
      if(column.label == '全選' || column.label == '全不選'){
        if(column.label == '全選'){
        column.label = '全不選'
        this.tableData.forEach((element, index) => {
          if(element.x_status == false || element.x_status == ''){
            element.x_status = true
          }
        });
        }else{
          column.label = '全選'
          this.tableData.forEach((element, index) => {
            if(element.x_status == true || element.x_status == ''){
              element.x_status = false
            }
          });
        }
      }
    },
    add:function()
    {
      this.isShow = true
    },
    del_batch:function()
    {
      var i = 0;
      var _self = this;
      _self.check_list.length = 0;

      this.tableData.forEach((element, index) => {
        if(element.x_status == true){
          element.row = index
          _self.check_list[i++] = element
        }
      });
      
      if(_self.check_list.length == 0){
        _self.$message.error('未勾選學生，請確認!!');
        return false;
      }                    

      for(var i = 0;i < _self.check_list.length;i++)
      {
        _self.tableData.forEach(function(item,index,array){
            if(_self.check_list[i].std_no == item.std_no){                      
              _self.tableData.splice(index,1)
            }
        })                                
      }
    },
    current_change(val) 
    {
      var _self = this;
      var start = ''
      var end = ''

      start = ((val - 1) * _self.pageSize) + 1;
      end = val * _self.pageSize
      _self.currentPage = val

    },
    size_change(val) 
    {
      var _self = this;
      var start = ''
      var end = ''
      start = ((_self.currentPage - 1) * val) + 1;
      end = _self.currentPage * val
      _self.pageSize = val
    },
    getshow: function (val) 
    {
      this.isShow = val.show;
    },
    getstudata:function(val)
    {
      this.tableData = this.tableData.concat(val) 
      const result = [];
      const map = new Map();
      for (const item of this.tableData) {
        if(!map.has(item.std_no)){
            map.set(item.std_no, true);
            result.push(item);
        }
      }
      this.tableData = result.sort(function (a, b) {
                          if (a.std_no > b.std_no) {
                            return 1
                          } else {
                            return -1
                          }
                      }
      )
   
      this.tableData.forEach(function(item,index,array){
        item.x_status = false
      })
      if(this.tableData.length > 0)
      {
        this.sch_no = this.tableData[0].sch_no
      }
    },
    cellmouseenter(row, column, cell, event)
    {
    },    
    download:function(res,val)
    {
      let _self = this
      let context = res.data
      let blob = new Blob([context])
      if("download" in document.createElement("a")){
          if (res.statusText == 'OK' || res.status == '200'){
                  let link = document.createElement("a")
                  link.download = val.row.file_name+'.'+val.row.file_extension
                  link.style.displya = "none"
                  //link.style.displya = "block"
                  link.href = URL.createObjectURL(blob)
                  document.body.appendChild(link)
                  link.click()
                  URL.revokeObjectURL(link.href)
                  document.body.removeChild(link)
          }else{
              _self.$message.error('檔案下載發生錯誤!!')
          }
      }else{
          navigator.msSaveBold(blob,filename)
      }
    },
  },
  components: {
    PubStuList
  },
  beforeDestroy(){

  },
  mounted() {
    /*
    let _self = this
    apiurl = _self.api_interface.list_file

    _self.$http({
    url: apiurl,
    method: 'get',
    headers:{'SkyGet':_self.$token}
    })
    .then((res) => {
      if (res.data.status == 'Y') {
        _self.tableData = res.data.dataset
      } else {
        _self.tableData = []
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
    .finally()
    */
  },
  beforeMount() {
    switch (this.userStatic.data_structure) {
      case 'StuTurnExport'://課程諮詢
        this.data_structure = data_structure.stu_turn_export
        break;
      default:
    }
  },
  computed: {
    render_header() {
      let headers = this.data_structure.file_header.filter((a) => { return a.hidden === 'N' })
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
.item {
  margin-top: 10px;
}
</style>
