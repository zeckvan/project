<template>
    <div>
      <v-pubquery></v-pubquery>
      <div align="left">
        <el-button type="primary" v-on:click="add">新增</el-button>
      </div>
      <el-table :data="tableData"
                stripe
                style="width: 100%">
                <el-table-column v-for="item in data_structure" 
                 :key = "item.prop"
                 :prop="item.prop"
                 :label="item.label"
                 :width="item.width">
                </el-table-column>
                <el-table-column
          fixed="right"
          label="功能"
          width="120">
          <template slot-scope="scope">
              <el-button type="text" size="small">查看</el-button>
              <el-button type="text" size="small">刪除</el-button>
          </template>
          </el-table-column>
      </el-table>
      <component :is='content' 
                 :dialog_show.sync="dialog_show" 
                 v-on:get-show="getshow" v-if="isShow">
      </component>      
<!--      
      <v-stucadreform :dialog_show.sync="dialog_show" v-on:get-show="getshow" v-if="isShow" ></v-stucadreform>                

      <el-table-column
        fixed
        type="index"
        width="50">
      </el-table-column>
        <el-table-column
          prop="a"
          label="學年"
          width="180">
        </el-table-column>
        <el-table-column
          prop="b"
          label="學期"
          width="180">
        </el-table-column>
        <el-table-column
          prop="c"
          label="單位名稱">
        </el-table-column>
        <el-table-column
          prop="d"
          label="幹部等級">
        </el-table-column>
        <el-table-column
          prop="e"
          label="開始日期">
        </el-table-column>
        <el-table-column
          prop="f"
          label="結束日期">
        </el-table-column>
-->
    </div>
  </template>
  
  <script type="module">
    import StucCdreForm from '@/components/student/stu_cadre_form.vue'
    import PubQuery from '@/components/pub/pub_query.vue'
    import * as data_structure from '@/js/stu_grid_structure.js'
    
    export default {
      props: {
              userStatic: {
                type: Object,
              },
          },      
      data() {
        return {
            content: 'v-stucadreform',
            data_structure:data_structure.stu_cadre,        
            tableData: [{
              a:'105',
              b:'1',
              c:'班長',
              d:'其它幹部',
              e:'105/01/01',
              f:'105/01/02'
            }, {
              a:'106',
              b:'1',
              c:'副班長',
              d:'其它幹部',
              e:'106/01/01',
              f:'106/01/02'
            },],
            isShow: false,
            dialog_show: true
        }
      },
      methods: {
        add:function(){
          this.isShow = true
        },
        getshow:function(val){
          this.isShow = val.show;
        }
      },
      components: {
          'v-stucadreform':StucCdreForm,
          'v-pubquery': PubQuery        
      },
      mounted: function () {
        console.log('zecktest',this.$router);
      },
    }
  </script>
  
  <style></style>
  