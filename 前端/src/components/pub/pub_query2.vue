<template>
  <div align="left">
    <el-form :inline="true" :model="fromyms">
      <el-form-item label="學年：">
        <datalistyear v-on:get-year="getyear" :year_id="year_id" />
      </el-form-item>
      <el-form-item label="學期:">
        <datalistsms v-on:get-sms="getsms" :sms_type="sms_type" :sms_id="sms_id" :isShowShort="true" />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="query">查詢</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
  
<script>
import datalistyear from '@/components/pub/DataList_year.vue'
import datalistsms from '@/components/pub/DataList_sms.vue'
export default {
  name: "PubQuery2",
  data() {
    return {
      fromyms: {
        year: "",
        sms: ""
      },
      sms_type: "2",
      year_id: "",
      sms_id: ""
    };
  },
  methods: {
    query: function () {
      this.$emit('get-yms', this.fromyms)
    },
    getyear: function (val) {
      this.fromyms.year = val
    },
    getsms: function (val) {
      this.fromyms.sms = val
    }
  },
  components: {
    datalistyear,
    datalistsms
  },
};
</script>
  
<style></style>
  