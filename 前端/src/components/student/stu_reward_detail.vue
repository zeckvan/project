<template>
  <div>
    <div align="left">
      <pub_query @get-yms="getReward" />
    </div>
    <el-table :data="tableData" stripe style="width: 100%">
      <el-table-column prop="phr_year" label="學年" width="180">
      </el-table-column>
      <el-table-column prop="phr_sms" label="學期" width="180">
      </el-table-column>
      <el-table-column prop="msp_h_dt" label="獎懲日期">
      </el-table-column>
      <el-table-column prop="mpwe_name" label="獎懲事實">
      </el-table-column>
      <el-table-column prop="reward" label="獎懲內容">
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
  import pub_query from '../pub/pub_query2.vue';

  export default {
    name: 'StuRewardDetail',
    data() {
      return {
        tableData: []
      }
    },
    methods: {
      getReward: function (val) {
        let _self = this;
        const apiurl = `${this.$apiroot}/stuphrRecord/${_self.$token}`   
        const loading = _self.$loading({
          lock: true,
          text: '資料讀取中，請稍後。',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        });

        _self.axios.get(apiurl, { params: { year: val.year, sms: val.sms },headers:{'SkyGet':_self.$token} })
          .then((res) => {
            if (res.data.status == 'Y') {
              _self.tableData = res.data.dataset;
            } else {
              _self.tableData = [];
              _self.$message.error('查無資料，請確認');
            }
          })
          .catch((error)=>{
            _self.tableData = [];
          })
          .finally(()=>loading.close());
      }
    },
    components: {
      pub_query
    }
  }
</script>

<style></style>
