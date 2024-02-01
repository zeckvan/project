<template>
    <div>    
        <el-select v-model="year" placeholder="學年" @change="change" :disabled="disabled">
            <el-option v-for="item in ymslist" :key="item.year_id" :label="item.year_id" :value="item.year_id" >
            </el-option>        
        </el-select>    
    </div>
</template>
  
<script>
export default {
  name: "yms_year",
  props: {
            year_id:{
                  type:String
                },
            disabled:{
                  type:Boolean
            },
      },    
  data() {
    return {
      year:'',
      ymslist:[]
    };
  },
  computed: {

  },
  methods: {
    change:function(val){
        this.$emit('get-year', val.toString())
      }
  },  
  mounted() {
      let _self = this
      const apiurl = `${_self.$apiroot}/s90yearinfo`
      _self.$http({
              url:apiurl,
              method:'get',
              headers:{'SkyGet':_self.$token}
              })
              .then((res)=>{        
                    _self.ymslist = res.data.dataset
                    if(res.data.dataset.length > 0){
                      _self.year = (this.year_id === "" ? res.data.dataset[0].year_id.toString():this.year_id)
                      //this.$emit('update:year_id', _self.year );
                      this.$emit('get-year',_self.year)
                    }                                        
                })         
              .catch((error)=>{
                       // _self.$message.error('呼叫後端【s90yearinfo】發生錯誤,'+error)
                      })
              .finally()                                       
  },
  beforeMount(){

  }   
};
</script>
  
<style></style>
  