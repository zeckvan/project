<template>
  <div>
    <PubQuery v-on:get-condition="getcondition"></PubQuery>
    <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="課程諮詢資料建立" name="first">
          <TeaConsultGrid
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="tableData"
            :total="pagetotal1"
            v-on:get-studata="getstudata"
          >
          </TeaConsultGrid>
        </el-tab-pane>
        <el-tab-pane label="課程諮詢學生" name="second">
          <TeaConsultStu
            :userStatic="userStatic"
            :api_interface="api_interface"
            :parentData="stuData"
            :total="pagetotal2"
            :complex_key="complex_key"
            :tea_consult="tea_consult"
          >
          </TeaConsultStu>
        </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
  var apiurl = ''
  import PubQuery from '@/components/pub/pub_query.vue'
  import TeaConsultGrid from '@/components/teacher/tea_consult_grid.vue'
  import TeaConsultStu from '@/components/teacher/tea_consult_stulist.vue'
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
                    parameter: {},
                    tableData:[],
                    stuData:[],
                    complex_key:{},
                    tea_consult:{}
                }
            },
    components: {
        PubQuery,
        TeaConsultGrid,
        TeaConsultStu
    },
    methods: {
        getstudata:function(val){
          this.tea_consult = val
          this.get_studata(val.a,val.b,val.c,val.d,val.e,1, 10)
        },
        handleClick:function(){
        },
        getcondition: function (val) {
          var _self = this;
          _self.year = val.year
          _self.sms = val.sms

          apiurl = _self.api_interface.get_data
          this.get_data(apiurl, val.year, val.sms, _self.currentPage, _self.pageSize)
        },
        get_data: function (apiurl, year, sms, start, end) {
          let _self = this
          const loading = _self.$loading({
            lock: true,
            text: '資料讀取中，請稍後。',
            spinner: 'el-icon-loading',
            background: 'rgba(0, 0, 0, 0.7)'
          });

          _self.parameter.emp_id = ''
          _self.parameter.year_id = year
          _self.parameter.sms_id = sms
          _self.parameter.sRowNun = start
          _self.parameter.eRowNun = end
          _self.parameter.sch_no = ''
          _self.parameter.token = _self.$token
  
          _self.$http({
            url: apiurl,
            method: 'get',
            params: _self.parameter,
            headers:{'SkyGet':_self.$token}
          })
          .then((res) => {
            if (res.data.status == 'Y') {
              _self.tableData = res.data.dataset
              _self.pagetotal1 = res.data.total
              _self.complex_key = res.data.dataset[0]
              this.tea_consult = res.data.dataset[0]
              _self.get_studata(res.data.dataset[0].a,res.data.dataset[0].b,res.data.dataset[0].c,res.data.dataset[0].d,res.data.dataset[0].e,start, end)

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
        },
        get_studata: function (sch_no,year, sms,emp_id,ser_id, start, end) {
          let _self = this
          apiurl = _self.api_interface.get_stuconsult
          _self.parameter.emp_id = emp_id
          _self.parameter.year_id = year
          _self.parameter.sms_id = sms
          _self.parameter.sRowNun = start
          _self.parameter.eRowNun = end
          _self.parameter.sch_no = sch_no
          _self.parameter.ser_id = ser_id

          _self.$http({
            url: apiurl,
            method: 'get',
            params: _self.parameter,
            headers:{'SkyGet':_self.$token}
          })
            .then((res) => {
              if (res.data.status == 'Y') {
                _self.stuData = res.data.dataset
                _self.pagetotal2 = res.data.dataset[0].x_cnt
              } else {
                _self.stuData = []
                _self.pagetotal2 = 0
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
        },
    },
    mounted: function () {

    }
  }
</script>
