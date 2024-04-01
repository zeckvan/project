<template>
  <div>
    <!--
    <el-form :inline="true" :model="formInline" align="left">
      <el-form-item label="學年：">      
        <datalistyear v-on:get-year="getyear" :year_id="year_id"/>          
      </el-form-item>
      <el-form-item label="學期:">
        <el-select v-model="sms_id" placeholder="學期">
            <el-option v-for="item in smslist" :key="item.id" :label="item.name" :value="item.name" >
            </el-option>        
        </el-select>    
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="query">查詢</el-button>
      </el-form-item>
    </el-form>
    -->
    <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="一年級" name="first">
          <OperateGrade
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="ParentData"
          >
          </OperateGrade>
        </el-tab-pane>
        <el-tab-pane label="二年級" name="second">
          <OperateGrade
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="ParentData"
          >
          </OperateGrade>
        </el-tab-pane>
        <el-tab-pane label="三年級" name="third">
          <OperateGrade
            :userStatic="userStatic"
            :api_interface="api_interface"
            :tableData="ParentData"
          >
          </OperateGrade>
        </el-tab-pane>        
    </el-tabs>
  </div>
</template>

<script>
  var apiurl = ''
  import datalistyear from '@/components/pub/DataList_year.vue'  
  import OperateGrade from '@/components/admin/OperateGrade.vue'
  import * as adminAPI from '@/apis/adminApi.js'

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
                    tempData: [],
                    ParentData:[],
                    formInline: {
                                  year: "111",
                                  sms: "1",
                                  type: ""
                                },
                    smslist:[
                              {id:'0',name:'0'},
                              {id:'1',name:'1'},
                              {id:'2',name:'2'}
                            ],
                    year_id:'',
                    sms_id:'0'
                }
            },
    components: {
        OperateGrade,
        datalistyear
    },
    methods: {
      query:function(){

      },
      getyear:function(val){

      },      
      handleClick:function(val){
        this.get_data(Number(val.index)+1)
      },
      get_data: async function (page_id) {
        try {
              let _self = this

              const { data, statusText } = await adminAPI.OperateGrade.Get() 

              if (statusText !== "OK") {
                throw new Error(statusText);
              }

              _self.tempData = data.dataset
              _self.ParentData = data.dataset.filter(function(item,index,array)
              {
               return item.grade_id == page_id
              })
          } catch (error) {
            
          }
      },
    },
    mounted: async function () {
      await this.get_data('1')
    }
  }
</script>
