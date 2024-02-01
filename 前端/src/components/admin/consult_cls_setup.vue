<template>
  <div>
    <el-container style="height:100%; border: 1px solid #eee">
      <el-main style="border: 1px solid #eee;width: 17%;">
        <consult_tea :userStatic="userStatic" :api_interface="api_interface" v-on:get-empdata="getempdata"/>
      </el-main>     
      <el-main style="border: 1px solid #eee;width: 14%;">
        <consult_cls 
                    :userStatic="userStatic" 
                    :api_interface="api_interface" 
                    :getempid="getempid" 
                    v-on:get-insertStatus="getinsertStatus"/>
      </el-main>    
      <el-main style="border: 1px solid #eee;width: 29%;">
        <consult_list :userStatic="userStatic" :api_interface="api_interface" :getAddStatus="getAddStatus"/>
      </el-main>          
    </el-container>
  </div>
</template>

<script type="module">
  var apiurl = ''
  import * as data_structure from '@/js/pub_grid_structure.js'
  import consult_tea from '@/components/admin/consult_cls_setup_tea.vue'
  import consult_cls from '@/components/admin/consult_cls_setup_cls.vue'
  import consult_list from '@/components/admin/consult_cls_setup_consult.vue'
  
  export default {
    props: {
      userStatic: {
        type: Object,
      },
      api_interface: {
        type: Object,
      },
    },
    data() {
      return {
        data_structure: {},
        teadataTemp:[],
        teadata:[],
        clsdata:[],
        consult:[],
        total: 0,
        currentPage: 1,
        pageSize: 10,
        filter_emp_id:'',
        filter_emp_name:'',
        show_page:true,
        getempid:'',
        getAddStatus:new Date()
      }
    },
    methods: {
      getempdata:function(val){
        this.getempid = val.row.emp_id
      },
      getinsertStatus:function(val){
        this.getAddStatus = new Date()
      },
    },
    components: {
      consult_tea,
      consult_cls,
      consult_list
    },
    beforeDestroy(){
    },
    mounted() {

    },
    beforeMount() {
      switch (this.userStatic.data_structure) {
        case 'consult_cls':
          this.data_structure = data_structure.consult_cls
          break;
        default:
      }
    },
    computed: {

    }
  }
</script>

<style scoped>

</style>
