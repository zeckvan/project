<template>
    <div id="app">
      <el-menu :default-active="activeIndex"
                  class="el-menu-demo"
                  mode="horizontal"
                  @select="handleSelect"
                  style="margin-top:-20px"
                  background-color="#545c64"
                  text-color="#fff"
                  active-text-color="#ffd04b"
                  router>
          <el-menu-item index="1" route="stubasic">基本資料</el-menu-item>
          <el-submenu index="2">
              <template slot="title">修課紀錄</template>
              <el-menu-item index="2-1" route="stuconsultQuery">課程諮詢紀錄</el-menu-item>
              <el-menu-item index="2-2" route="stuattestation">上傳課程學習成果</el-menu-item>
              <el-menu-item index="2-3" route="stuattestationconfirm">學習成果認證</el-menu-item>
              <el-menu-item index="2-4" route="stuattestationcentraldb">勾選課程學習成果</el-menu-item>
              <el-menu-item index="2-5">課程學習成果提交紀錄</el-menu-item>
          </el-submenu>
          <el-submenu index="3">
              <template slot="title">多元表現</template>
              <el-menu-item index="3-1" route="stucadre">幹部經歷紀錄</el-menu-item>
              <el-menu-item index="3-2" route="stucompetition">競賽參與紀錄</el-menu-item>
              <el-menu-item index="3-3" route="stulicense">檢定證照紀錄</el-menu-item>
              <el-menu-item index="3-4" route="stuvolunteer">服務學習紀錄</el-menu-item>
              <el-menu-item index="3-5" route="sturesult">作品成果紀錄</el-menu-item>
              <el-menu-item index="3-6" route="stustudyf">彈性學習時間紀錄</el-menu-item>
              <el-menu-item index="3-7" route="stugroup">團體活動時間紀錄</el-menu-item>
              <el-menu-item index="3-8" route="stuworkplace">職場學習紀錄</el-menu-item>
              <el-menu-item index="3-9" route="stucollege">大學及技專校院先修課程紀錄</el-menu-item>
              <el-menu-item index="3-10" route="stuother">其他多元表現紀錄</el-menu-item>
              <el-menu-item index="3-11" route="studiversecheck">勾選多元表現</el-menu-item>
              <el-menu-item index="3-12">多元表現提交紀錄</el-menu-item>
          </el-submenu>
          <el-submenu index="4">
              <template slot="title">課程諮詢教師</template>
              <el-menu-item index="4-1" route="teaconsultquery">查詢學生學習成果及多元表現概況</el-menu-item>
              <el-menu-item index="4-2" route="teaconsult">學生諮詢課程填寫</el-menu-item>
          </el-submenu>
          <el-submenu index="5">
              <template slot="title">中央資料庫</template>
              <el-menu-item index="5-1" route="centerDB-stdquery">查詢提交記錄</el-menu-item>
              <el-menu-item index="5-2" route="centerDB-stdfeedback">問題回報</el-menu-item>
          </el-submenu>      
          <el-submenu index="6">
              <template slot="title">授課教師</template>
              <el-menu-item index="6-1" route="teaattestationquery"> 授課班級資料</el-menu-item>
              <el-menu-item index="6-2" route="teaattestation">學習成果認證</el-menu-item>
          </el-submenu>   
          <el-submenu index="7">
              <template slot="title">班級導師</template>
              <el-menu-item index="7-1" route="teatutor">查詢導師班學生學習果及多元學習</el-menu-item>
          </el-submenu>              
          <el-submenu index="99">
              <template slot="title">系統管理者</template>
              <el-submenu index="99-1">
                <template slot="title">系統管理</template>
                <el-menu-item index="99-1-1" route="SystemSetup">設定功能參數</el-menu-item>
                <el-menu-item index="99-1-2" route="OperateSetUp">設定學校作業時間</el-menu-item>
                <el-menu-item index="99-1-3" route="consultSetUp">設定課程諮詢教師</el-menu-item>
              </el-submenu>   
              <el-submenu index="99-2">
                <template slot="title">查詢</template>
                <el-menu-item index="99-2-1" route="LearningResultView">查詢學生學習成果勾選情形</el-menu-item>
                <el-menu-item index="99-2-2" route="MultipleLearningView">查詢學生多元表現勾選情形</el-menu-item>
                <el-menu-item index="99-2-3" route="AttestationNotYet">查詢教師未認證狀況</el-menu-item>
              </el-submenu>   
              <el-submenu index="99-3">
                <template slot="title">匯出名冊</template>
                <el-menu-item index="99-3-1" route="ExportExcel">匯出提交名冊</el-menu-item>
                <el-menu-item index="99-3-2" route="CenterDB">收訖明細</el-menu-item>            
              </el-submenu>        
              <el-submenu index="99-4">
                <template slot="title">學習歷程資料轉出及轉入</template>
                <el-menu-item index="99-4-1" route="StuTurnView">學習歷程資料轉出/轉入</el-menu-item>
              </el-submenu>                                                       
          </el-submenu>                 
      </el-menu>    
    </div>
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
                      activeIndex:"0"
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
          handleSelect:function(key, keyPath){
                     // console.log(key,keyPath)
                     this.content = key
                     this.isShow = true
                  }
      },
      mounted: function () {

      }
    }
  </script>
