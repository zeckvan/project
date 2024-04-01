<template>
    <div>    
        <el-select v-model="year" placeholder="學年" @change="change" :disabled="disabled">
            <el-option v-for="item in ymslist" :key="item.year_id" :label="item.year_id" :value="item.year_id" >
            </el-option>        
        </el-select>    
    </div>
</template>
  
<script>
import * as adminAPI from  '@/apis/adminApi.js' 
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
  async mounted() {     
      let _self = this

      const { data, statusText } = await adminAPI.s90yearinfo.Get()

      if (statusText !== "OK") {
          throw new Error(statusText);
      }

      _self.ymslist = data.dataset
      if(data.dataset.length > 0){
        _self.year = (this.year_id === "" ? data.dataset[0].year_id.toString():this.year_id)
        this.$emit('get-year',_self.year)
      }                                
  },
  beforeMount(){

  }   
};
</script>
  
<style></style>
  