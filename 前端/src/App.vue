<template>
  <div id="app">
<Menu/>
    <router-view style="margin-top:15px" :key="$route.path"></router-view>
  </div>
</template>

<script>
  import StuBasic from '@/components/student/stu_basic.vue'
  import StuAddendQuery from '@/components/student/stu_addend_query.vue'
  import StuAddendDetail from '@/components/student/stu_addend_detail.vue'
  import StuRewardDetail from '@/components/student/stu_reward_detail.vue'
  import PubQuery from '@/components/pub/pub_query.vue'
  import Menu from '././views/MenuView.vue'

  export default {
    name: 'StuBasicView',
    data: function () {
                return {
                    activeIndex:"0",
                    first:"N"
                }
            },
    components: {
        StuBasic,
        StuAddendQuery,
        StuAddendDetail,
        StuRewardDetail,
        Menu,
        'v-pubquery': PubQuery

    },
    methods: {
        handleSelect:function(key, keyPath){
                   this.content = key
                   this.isShow = true
                }
    },
    mounted: function () {

    },
    created:function(){

    }
  }
</script>

<style lang="scss">
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

nav {
  padding: 30px;

  a {
    font-weight: bold;
    color: #2c3e50;

    &.router-link-exact-active {
      color: #42b983;
    }
  }
}

.el-table__body .el-table__row.hover-row td {
        background-color:#ffd04b !important;
        cursor: pointer;
      }
.el-table tbody tr:hover>td {
  background-color:#ffd04b !important;
  cursor: pointer;
}
</style>
