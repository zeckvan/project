<template>
  <div>
    <div align="left">

    </div>
    <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="學習歷程資料匯出" name="first">
          <StuExport
          :userStatic="userStatic"
          :api_interface="api_interface"/>
        </el-tab-pane>
        <el-tab-pane label="學習歷程資料檔案下載" name="second">
          <StuExportList
          :userStatic="userStatic"
          :api_interface="api_interface"
          :tableData="tempData"/>
        </el-tab-pane>
        <el-tab-pane label="學習歷程資料匯入" name="third">
          <StuImport
          :userStatic="userStatic"
          :api_interface="api_interface"/>
        </el-tab-pane>    
    </el-tabs>
  </div>
</template>

<script>
  var apiurl = ''
  import StuExport from '@/components/student/stu_turn_export.vue'
  import StuExportList from '@/components/student/stu_turn_export_file.vue'
  import StuImport from '@/components/student/stu_turn_import.vue'
  export default {
    name: 'StuTurnView',
    props: {
        userStatic: {
          type: Object,
        },
        api_interface: {
          type: Object,
        },
    },
    data: function () {
                return {
                    activeName: 'first',
                    activePage:'0',
                    tempData:[],
                }
            },
    components: {
      StuExport,
      StuExportList,
      StuImport
    },
    methods: {
      handleClick:function(tab, event)
      {
        if(tab.index == '1')
        {
          let _self = this
          var apiurl = this.api_interface.list_file
          _self.$http({
          url: apiurl,
          method: 'get',
          headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
            if (res.data.status == 'Y') {
              _self.tempData = res.data.dataset
            } else {
              _self.tempData = []
            }
          })
          .catch((error) => {
              _self.tempData = []
              _self.$message({
                message: '系統發生錯誤'+error,
                type: 'error',
                duration:0,
                showClose: true,
              })
            })
          .finally()

        }
      }
    },                  
    mounted: function () {

    },
    beforeDestroy(){

    },
    mounted() {

    },    
  }
</script>
<style>

</style>
