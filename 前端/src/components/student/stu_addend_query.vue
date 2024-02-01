<template>
  <el-form :inline="true" :model="formInline">
    <el-form-item label="學年：">
      <datalistyear v-on:get-year="getyear" :year_id="year_id" />
    </el-form-item>
    <el-form-item label="學期：">
      <datalistsms v-on:get-sms="getsms" :sms_id="sms_id" />
    </el-form-item>
    <el-form-item label="假別：">
      <el-select v-model="formInline.type">
        <el-option v-for="item in mat_list" :key="item.id" :label="item.name" :value="item.id" >
        </el-option>
      </el-select>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" v-on:click="query">查詢</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import datalistyear from '@/components/pub/DataList_year.vue'
import datalistsms from '@/components/pub/DataList_sms.vue'  
export default {
  name: 'StuAddendQuery',
  props: {
           yms: {
                type: Array
            },
            yms_year:{
              type:String
            },
            formInline:{
              type:Object
            }
        },
  data() {
    return {
      year_id:"",
      sms_id:"",
      mat_list:[
              {id:'%',name:'所有假別'},
              {id:'1',name:'防疫假'},
              {id:'A',name:'曠課'},
              {id:'B',name:'早曠'},
              {id:'G',name:'遲到'},
              {id:'H',name:'事假'},
              {id:'I',name:'病假'},
              {id:'J',name:'公假'},
              {id:'K',name:'喪假'},
              {id:'T',name:'生理假'},
              {id:'U',name:'病健'},
            ]
    }
  },
  computed: {
  },
  methods: {
    query:function(){                                
      this.$emit('get-form', this.formInline)
    },
    getyear:function(val){
      this.formInline.year = val
    },    
    getsms:function(val){
      this.formInline.sms = val
    },    
  },
  beforeCreate(){
  },
  mounted() {

  },
  beforeMount(){
   
  },
  created(){
    //console.log(yms_year)
  },
    components: {
      datalistyear,
      datalistsms    
      },  
}
</script>

<style></style>
