<template>
    <div>
      <el-select v-model="sms" placeholder="學期" @change="change" :disabled="disabled">
        <el-option v-for="item in smslist" :key="item.sms_id" :label=" sms_type == '1' ? item.sms_name : item.sms_abr " :value="item.sms_id" >
        </el-option>        
      </el-select>    
    </div>
</template>
  
<script>
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
  mounted() {
    let _self = this
      
      const apiurl = `${_self.$apiroot}/s90smsinfo`
      _self.$http({
              url:apiurl,
              method:'get',
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{
                    if(_self.$props.isShowShort){
                      _self.smslist = res.data.dataset.filter(function(value) {
                        return value.sms_id == 1 || value.sms_id == 2;
                      });
                    }
                    else
                    {
                      _self.smslist = res.data.dataset
                    }
                    if(res.data.dataset.length > 0){
                      _self.sms = (this.sms_id === "" ? res.data.dataset[0].sms_id.toString():this.sms_id)
                      this.$emit('get-sms', _self.sms)
                    }                                        
                })         
              .catch((error)=>{
                        _self.$message.error('呼叫後端【s90smsinfo】發生錯誤,'+error)
                      })
              .finally()                  
  }    
};
</script>
  
<style></style>
  