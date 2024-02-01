<template>
      <el-select v-model="year" placeholder="學年期" @change="change">
        <el-option v-for="item in ymslist" :key="item.yms_year" :label="item.yms_year" :value="item.yms_year" >
        </el-option>        
      </el-select>    
</template>
    
<script>
  export default {
    name: "yms",
    props: {
        },    
    data() {
      return {
        year:"",
        ymslist:[]
      };
    },
    computed: {

    },
    methods: {
      change:function(){
            this.$emit('get-year', this.year)
        }
    },
    mounted() {
        let _self = this

        const apiurl = `${_self.$apiroot}/s90ymsinfo/All`
   
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
                      _self.ymslist = yms_year
                      if(yms_year.length > 0){
                        _self.year = yms_year[0].yms_year.toString()
                        this.$emit('get-year', _self.year)
                      }
                      
                  })         
                .catch((error)=>{
                          _self.$message.error('呼叫後端【s90ymsinfo】發生錯誤,'+error)
                        })
                .finally()                  
    }    
  };
</script>
    
<style></style>
    