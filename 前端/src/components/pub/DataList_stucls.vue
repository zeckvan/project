<template>
    <div>
      <el-select v-model="cls" placeholder="" @change="change" :disabled="disabled" @focus="focus">
        <el-option v-for="item in clslist" :key="item.cls_id" :label=" item.cls_abr" :value="item.cls_id" >
        </el-option>        
      </el-select>    
    </div>
</template>
  
<script>
export default {
  name: "stu_cls",
  props: {
        cls_abr:{
          type: String
        },
        cls_id:{
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
      cls:"",
      clslist:[]
    };
  },
  computed: {

  },
  methods: {
    focus:function(){    
      /*  
      this.obj.year_id = this.year_id
      this.obj.sms_id = this.sms_id
      this.$emit('get-cls', this.cls)      
      this.getData(this.obj)
      */
    },
    change:function(val){
          this.$emit('get-cls',val.toString())
      },
    getData:function(){
      const apiurl = `${_self.$apiroot}/S04_stucls`
      _self.$http({
              url:apiurl,
              method:'get',
              params:obj,
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{                   
                    _self.clslist = [],
                    _self.cls = '' 
                    if(res.data.status == 'Y'){
                      _self.cls = (this.cls_id === "" ? res.data.dataset[0].cls_id.toString():this.cls_id)
                      this.$emit('get-cls', _self.cls)
                    }else{
                      this.$emit('get-cls', _self.cls)
                    }                              
                })         
              .catch((error)=>{
                        _self.$message.error('呼叫後端【S04_stucls】發生錯誤,'+error)
                      })
              .finally()             
    }
  },
  mounted() {
    this.getData()
  }
};
</script>
  
<style></style>
  