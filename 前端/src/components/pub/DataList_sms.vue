<template>
    <div>
      <el-select v-model="sms" placeholder="學期" @change="change" :disabled="disabled">
        <el-option v-for="item in smslist" :key="item.sms_id" :label=" sms_type == '1' ? item.sms_name : item.sms_abr " :value="item.sms_id" >
        </el-option>        
      </el-select>    
    </div>
</template>
  
<script>
import * as adminAPI from  '@/apis/adminApi.js' 
export default {
  name: "yms_sms",
  props: {
        sms_type:{
          type: String
        },
        sms_id:{
          type: String
        },
        disabled:{
              type:Boolean
        },
        isShowShort:{
          type:Boolean
        }
      },    
  data() {
    return {
      sms:"",
      smslist:[]
    };
  },
  computed: {

  },
  methods: {
    change:function(val){
          this.$emit('get-sms',val.toString())
      }
  },
  async mounted() {
      let _self = this
          
      const { data, statusText } = await adminAPI.s90smsinfo.Get()

      if (statusText !== "OK") {
            throw new Error(statusText);
      }

      if(_self.$props.isShowShort){
          _self.smslist = data.dataset.filter(function(value) {
            return value.sms_id == 1 || value.sms_id == 2;
          });
      }
      else
      {
        _self.smslist = data.dataset
      }

      if(data.dataset.length > 0){
        _self.sms = (this.sms_id === "" ? data.dataset[0].sms_id.toString():this.sms_id)
        this.$emit('get-sms', _self.sms)
      }        
  }    
};
</script>
  
<style></style>
  