<template>
  <div>
    <PubQuery v-on:get-condition="getcondition"></PubQuery>
    <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="審查課程學習成果通過/不通過" name="first">
          <TeaAttestationGrid
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="tableVerify"
            :total="pagetotal1"
            :activePage="activePage"
            :parameter="parameter"
            :operate_state="operate_state"
          >
          </TeaAttestationGrid>
        </el-tab-pane>
        <el-tab-pane label="發佈課程學習成果" name="second">
          <TeaAttestationGrid
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="tableRelease"
            :total="pagetotal1"
            :activePage="activePage"
            :parameter="parameter"
            :operate_state="operate_state"
          >
          </TeaAttestationGrid>
        </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
  var apiurl = ''
  import PubQuery from '@/components/pub/pub_query_grade.vue'
  import TeaAttestationGrid from '@/components/teacher/tea_attestation_grid.vue'
  import * as std_structure from '@/js/stu_grid_structure.js'
  export default {
    name: 'TeaConsultView',
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
                    pagetotal1: 0,
                    pagetotal2: 0,
                    currentPage: 1,
                    pageSize: 10,
                    parameter: {year_id:'',sms_id:'',grade_id:''},
                    tableData:[],
                    tableVerify:[],
                    tableRelease:[],
                    stuData:[],
                    complex_key:{},
                    tea_consult:{},
                    activePage:'0',
                    operate_state:{open_yn:'Y',startdate:'',enddate:''}
                }
            },
    components: {
        PubQuery,
        TeaAttestationGrid
    },
    methods: {
        handleClick:function(tab, event){
          this.activePage = tab.index
        },
        getcondition: function (val) {
          this.parameter.year_id = val.year
          this.parameter.sms_id = val.sms
          this.parameter.grade_id = val.grade_id

          if(this.activePage == '0'){
            this.getdata1(this.parameter)
          }else{
            this.getdata2(this.parameter)
          }
          this.getOpStatus(val)
        },
        getdata1:function(val){
          let _self = this
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          _self.parameter.emp_id = ''
          _self.parameter.year_id = val.year_id
          _self.parameter.sms_id = val.sms_id
          _self.parameter.sRowNun = 1
          _self.parameter.eRowNun = 999
          _self.parameter.sch_no = ''
          _self.parameter.token = _self.$token
          
          _self.$http({
            url: _self.api_interface.get_Verify,
            method: 'get',
            params: _self.parameter,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
              if (res.data.status == 'Y') {
                _self.tableVerify = res.data.dataset
                _self.tableVerify.forEach(function(item,index,array){
                        if(item.v == 'Y'){ 
                              item.x_status = "已發佈"
                        }else{
                          item.x_status = false
                        }                      
                    })  
                _self.total = res.data.dataset[0].x_cnt
              } else {
                _self.tableVerify = []
                _self.$message.error('查無資料，請確認')
              }
          })
          .catch((error) => {
                _self.tableVerify = []
                _self.$message({
                  message: '系統發生錯誤'+error,
                  type: 'error',
                  duration:0,
                  showClose: true,
                })
          })
          .finally(() => loading.close())
        },
        getdata2:function(val){
          let _self = this
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          _self.parameter.emp_id = ''
          _self.parameter.year_id = val.year_id
          _self.parameter.sms_id = val.sms_id
          _self.parameter.sRowNun = 1
          _self.parameter.eRowNun = 999
          _self.parameter.sch_no = ''
          _self.parameter.token = _self.$token

          _self.$http({
            url: _self.api_interface.get_Release,
            method: 'get',
            params: _self.parameter,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
              if (res.data.status == 'Y') {
                _self.tableRelease = res.data.dataset
                _self.tableRelease.forEach(function(item,index,array){
                        if(item.v == 'Y'){ 
                              item.x_status = "已發佈"
                        }else{
                          item.x_status = false
                        }                      
                    })  
                _self.total = res.data.dataset[0].x_cnt
              } else {
                _self.tableRelease = []
                _self.$message.error('查無資料，請確認')
              }
          })
          .catch((error) => {
                _self.tableVerify = []
                _self.$message({
                  message: '系統發生錯誤'+error,
                  type: 'error',
                  duration:0,
                  showClose: true,
                })
          })
          .finally(() => loading.close())
        },
        getOpStatus:function(val)
        {
          let parameter = ''
          apiurl = this.api_interface.get_OpOpenYN
          parameter=
          {
              year_id:val.year,
              sms_id:val.sms,
              grade_id:val.grade_id,
              type_id:'02',
              token:this.$token,
              kind_id:'tea'
          }
          this.operate_state = std_structure.getOpenOpYN(this,apiurl,parameter,this.$token) 
        }
    },
    mounted: function () {

    }
  }
</script>
