<template>
    <el-tabs v-model="activeName" @tab-click="handleClick">
        <el-tab-pane label="個人資料" name="first">
            <StuBasic/>
        </el-tab-pane>
        <el-tab-pane label="出缺勤資料" name="second">
            <!--
                <StuAddendQuery/>
            -->
            <StuAddendDetail/>
        </el-tab-pane>
        <el-tab-pane label="獎懲記錄" name="third">
            <StuRewardDetail/>
        </el-tab-pane>
    </el-tabs>
</template>
  
<script>
  import StuBasic from '@/components/student/stu_basic.vue'
  import StuAddendQuery from '@/components/student/stu_addend_query.vue'
  import StuAddendDetail from '@/components/student/stu_addend_detail.vue'
  import StuRewardDetail from '@/components/student/stu_reward_detail.vue'
  import PubQuery from '@/components/pub/pub_query.vue'

  export default {
    name: 'StuBasicView',
    data: function () {
                return {
                    activeName: 'first',
                    basicform:{
                        std_no: '',
                        std_name: '',
                        std_identity: '',
                        std_birth_dt: '',
                        std_email: '',
                        std_fb: '',
                        std_ig:'',
                        std_aka:'',
                        std_memo:''                        
                    }
                }
            },    
    components: {
        StuBasic,
        StuAddendQuery,
        StuAddendDetail,
        StuRewardDetail,
        'v-pubquery': PubQuery        
    },
    methods: {
        handleClick:function(){
        }
    },
    mounted: function () {

    }   
  }
</script>
  