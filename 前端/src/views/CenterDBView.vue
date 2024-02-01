<template>
    <el-tabs v-model="activeName" @tab-click="">
        <el-tab-pane label="匯入收訖" name="1">
            <Upload v-if="activeName=='1'" :urlObject="urlObject"/>
        </el-tab-pane>
        <el-tab-pane label="設定開放" name="2">
            <DateSetup v-if="activeName=='2'" :urlObject="urlObject" />
        </el-tab-pane>
        <el-tab-pane label="明細查看" name="3">
            <QueryStd v-if="activeName=='3'" :urlObject="urlObject" />
        </el-tab-pane>
        <el-tab-pane label="問題回報紀錄查看" name="4">
            <Feedback v-if="activeName=='4'" :urlObject="urlObject" />
        </el-tab-pane>
        <el-tab-pane label="資料匯總" name="5">
            <QueryCount v-if="activeName=='5'" :urlObject="urlObject" />
        </el-tab-pane>
    </el-tabs>
</template>
  
<script>
  import Upload from '@/components/centerDB/manager/centerDB_upload.vue'
  import DateSetup from '@/components/centerDB/manager/centerDB_datetimesetup.vue'
  import QueryStd from '@/components/centerDB/centerDB_query_std.vue'
  import Feedback from '@/components/centerDB/manager/centerDB_answer_feedback.vue'
  import QueryCount from '@/components/centerDB/manager/centerDB_query_count.vue'

  export default {
    name: 'CenterDBView',
    data: function () {
        return {
            activeName: '1',
            urlObject:{
                pubYear:'/s90yearinfo',
                pubSms:'/s90smsinfo',
                Upload:'/CentralDBofLearningHistory/Import',
                DateSetup:'/CentralDBofLearningHistory/GetDateTimeSetup',
                QueryStd: '/CentralDBofLearningHistory/GetClsStd',
                Feedback:'/CentralDBofLearningHistory/FeedBack',
                FeedBackKindCls:'/CentralDBofLearningHistory/FeedBackKindCls',
                QueryCount:'/CentralDBofLearningHistory/QueryCount',
                GetCls:'/CentralDBofLearningHistory/GetCls',
                ymsString: this.ymsString,
                ymsOption: [],
            }
        }
    },
    components: {
        Upload,
        DateSetup,
        QueryStd,
        Feedback,
        QueryCount
    },
    methods: {
        ymsString(val) {
            let y = ""
            let s = ""
            if (val.length > 3) {
                y = val.substr(0, 3)
                s = val.substr(3, 1)
            }
            else {
                y = val.substr(0, 2)
                s = val.substr(2, 1)
            }
            return `${y}學年${s}學期`
        },
        getYms() {
            const _self = this
            const apiurl = `${this.$apiroot}/CentralDBofLearningHistory/GetYms`

            this.axios.get(apiurl,{headers:{'SkyGet':_self.$token}})
            .then((res) => {
                if (res.data.status == 'Y') {
                    _self.urlObject.ymsOption = res.data.dataset
                }
            })

        },
    },
    async beforeMount(){
        await this.getYms()
    },
    mounted() {
    }
  }
</script>
  