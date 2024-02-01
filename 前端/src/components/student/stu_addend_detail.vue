<template>
  <div>
    <div align="left">
      <StuAddendQuery v-on:get-form="getForm" :yms="yms" :yms_year="yms_year" :formInline="formInline"/>
    </div>
    <el-table :data="tableData"
              stripe
              style="width: 100%">
      <el-table-column
        prop="mat_year"
        label="學年"
        width="180">
      </el-table-column>
      <el-table-column
        prop="mat_sms"
        label="學期"
        width="180">
      </el-table-column>
      <!--
      <el-table-column
        prop="grd_id"
        label="年級">
      </el-table-column>
      <el-table-column
        prop=""
        label="班別">
      </el-table-column>
       -->
      <el-table-column
        prop="abs_sdt"
        label="缺勤時間">
      </el-table-column>
      <el-table-column
        prop="mat_name"
        label="假別">
      </el-table-column>
      <el-table-column
        prop="mat_sum"
        label="請假節數">
      </el-table-column>
    </el-table>
  </div>
</template>
  
  <script>
  import StuAddendQuery from '@/components/student/stu_addend_query.vue'

  export default {
    name: 'StuAddendDetail',
    data() {
      return {
          tableData: [],
          yms:[],
          yms_year:"",
          formInline: {
                        year:'',
                        sms: '',
                        type: '%'
                      }          
      }
    },
    methods: {
      formatDate: function (date) {
            var d = new Date(date),
                            month = '' + (d.getMonth() + 1),
                            day = '' + d.getDate(),
                            year = d.getFullYear();
                            
                        if (month.length < 2) 
                            month = '0' + month;
                        if (day.length < 2) 
                            day = '0' + day;
                return [year, month, day].join('-')
            },      
      getForm:function(val){    
        console.log('zeck1',val)    
        let _self = this
            
        const apiurl = `${this.$apiroot}/absenceRecord/${_self.$token}`         
        const loading = _self.$loading({
							  lock: true,
							  text: '資料讀取中，請稍後。',
							  spinner: 'el-icon-loading',
							  background: 'rgba(0, 0, 0, 0.7)'
							});	

        _self.$http({
                url:apiurl,
                method:'get',
                params:{                  
                  year:val.year,
                  sms:val.sms,
                  mat_id:val.type
                },
                headers:{'SkyGet':_self.$token}
                })
                .then((res)=>{
                                loading.close();
                                if (res.data.status == 'Y'){                                  
                                  res.data.dataset.forEach(function(item, index, array){
                                    item.abs_sdt = _self.formatDate(item.abs_sdt)                                        
                                  });
                                  _self.tableData = res.data.dataset
                                }else{
                                    _self.tableData = []
                                    _self.$message.error('查無資料，請確認')
                                }     
                        })        
                  .catch((error)=>{
                                    _self.tableData = []
                                    console.log(error)
                        })
                  .finally(()=>loading.close())            
      }
    },    
    components: {
        StuAddendQuery        
    },
    mounted() {
        let _self = this
        
        const apiurl = `${this.$apiroot}/s90ymsinfo/All`
   
        _self.$http({
                url:apiurl,
                method:'get',
                headers:{'SkyGet':_self.$token}
                })
                .then((res)=>{        
                      const yms_year = Array.from(new Set(res.data.dataset.map(s => s.yms_year)))   
                      .map(yms_year =>{
                        return {
                          yms_year:res.data.dataset.find(s=>s.yms_year === yms_year).yms_year
                        }
                      })                      
                      _self.yms = yms_year
                      if(yms_year.length > 0){
                        _self.formInline.year = yms_year[0].yms_year.toString()
                      }
                      
                  })         
                  .catch((error)=>{
                          _self.$message.error('呼叫後端【s90ymsinfo】發生錯誤,'+error)
                        })
                  .finally()                  
  },
  }
  </script>
  
  <style></style>
  