<template>
    <el-tabs v-model="activeName" @tab-click="">
        <el-tab-pane label="多元表現" name="1">
            <template v-if="activeName=='1'">
                <el-form :inline="true" :model="MultipleLearning" class="demo-form-inline">
                    <el-form-item label="學年：">
                        <selectYear @get-year="MultipleLearningGetyear" :year_id="MultipleLearning.year_id"/>
                    </el-form-item>
                    <el-form-item label="年級：">
                        <el-select v-model="MultipleLearning.grd_id" @change="MultipleLearningChangeGrdId" placeholder="請選擇">
                            <el-option label="一年級" value="1"></el-option>
                            <el-option label="二年級" value="2"></el-option>
                            <el-option label="三年級" value="3"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="類別：" v-if="MultipleLearning.grd_id==3">
                        <el-select v-model="MultipleLearning.type" placeholder="請選擇">
                            <el-option label="第一梯次多元表現" value="35"></el-option>
                            <el-option label="第二梯次多元表現" value="75"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="MultipleLearningExportExcel">匯出</el-button>
                    </el-form-item>
                </el-form>
            </template>
        </el-tab-pane>
        <el-tab-pane label="學習成果" name="2">
            <template v-if="activeName=='2'">
                <el-form :inline="true" :model="LearningResult" class="demo-form-inline">
                    <el-form-item label="學年：">
                        <selectYear @get-year="LearningResultGetyear" :year_id="LearningResult.year_id"/>
                    </el-form-item>
                    <el-form-item label="年級：">
                        <el-select v-model="LearningResult.grd_id" @change="LearningResultChangeGrdId" placeholder="請選擇">
                            <el-option label="一年級" value="1"></el-option>
                            <el-option label="二年級" value="2"></el-option>
                            <el-option label="三年級" value="3"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="類別：">
                        <el-select v-if="LearningResult.grd_id!=3" v-model="LearningResult.type" placeholder="請選擇">
                            <el-option label="學生課程學習成果" value="26"></el-option>
                            <el-option label="進修部學生課程學習成果" value="28"></el-option>
                        </el-select>
                        <el-select v-else-if="LearningResult.grd_id==3" v-model="LearningResult.type" placeholder="請選擇">
                            <el-option label="第一梯次學生課程學習成果" value="36"></el-option>
                            <el-option label="第二梯次學生課程學習成果" value="76"></el-option>
                            <el-option label="第一梯次進修部學生課程學習成果" value="38"></el-option>
                            <el-option label="第二梯次進修部學生課程學習成果" value="78"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="LearningResultExportExcel">匯出</el-button>
                    </el-form-item>
                </el-form>
            </template>
        </el-tab-pane>
        <el-tab-pane label="幹部" name="3">
            <template v-if="activeName=='3'">
                <el-form :inline="true" :model="StdPositionFromSch" class="demo-form-inline">
                    <el-form-item label="學年：">
                        <selectYear @get-year="StdPositionFromSchGetyear" :year_id="StdPositionFromSch.year_id"/>
                    </el-form-item>
                    <el-form-item label="學期：">
                        <selectSms @get-sms="StdPositionFromSchGetsms" :sms_id="StdPositionFromSch.sms_id" :isShowShort="true" />
                    </el-form-item>
                    <el-form-item label="年級：">
                        <el-select v-model="StdPositionFromSch.grd_id" @change="StdPositionFromSchChangeGrdId" placeholder="請選擇">
                            <el-option label="一年級" value="1"></el-option>
                            <el-option label="二年級" value="2"></el-option>
                            <el-option label="三年級" value="3"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item label="類別：" v-if="StdPositionFromSch.grd_id==3">
                        <el-select v-model="StdPositionFromSch.type" placeholder="請選擇">
                            <el-option label="第一梯次校內幹部" value="34"></el-option>
                            <el-option label="第二梯次校內幹部" value="74"></el-option>
                        </el-select>
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="StdPositionFromSchExportExcel">匯出</el-button>
                    </el-form-item>
                </el-form>
            </template>
        </el-tab-pane>
    </el-tabs>
</template>
  
<script>
  import selectYear from "@/components/pub/DataList_year.vue"
  import selectSms from "@/components/pub/DataList_sms.vue"

  export default {
    name: 'ExportExcel',
    data: function () {
        return {
            activeName: '1',
            urlObject:{
                MultipleLearning:"/ExportExcel/MultipleLearning",
                LearningResult:"/ExportExcel/LearningResult",
                StdPositionFromSch:"/ExportExcel/StdPositionFromSch",
            },
            MultipleLearning:{
                year_id:null,
                grd_id:null,
                type:"25",
                token:this.$token
            },
            LearningResult:{
                year_id:null,
                grd_id:null,
                type:null,
                token:this.$token
            },
            StdPositionFromSch:{
                year_id:null,
                sms_id:null,
                grd_id:null,
                type:"24",
                token:this.$token
            },
        }
    },
    components: {
        selectYear,
        selectSms
    },
    methods: {
        saveAs(res, type) {
            const {headers, data} = res

            let filename = ""
            switch(type){
                case "24":
                    filename = "校內幹部經歷名冊"
                    break
                case "25":
                    filename = "多元學習表現"
                    break
                case "26":
                    filename = "學生課程學習成果"
                    break
                case "28":
                    filename = "進修部學生課程學習成果"
                    break
                case "34":
                    filename = "第一梯次高三校內幹部經歷名冊"
                    break
                case "35":
                    filename = "第一梯次多元學習表現"
                    break
                case "36":
                    filename = "第一梯次學生課程學習成果"
                    break
                case "38":
                    filename = "第一梯次進修部學生課程學習成果"
                    break
                case "74":
                    filename = "第二梯次高三校內幹部經歷名冊"
                    break
                case "75":
                    filename = "第二梯次多元學習表現"
                    break
                case "76":
                    filename = "第二梯次學生課程學習成果"
                    break
                case "78":
                    filename = "第二梯次進修部學生課程學習成果"
                    break
            }

            filename += ".xlsx"
            let blob = new Blob([data], {type: headers["content-type"]})

            if("download" in document.createElement("a")){
                let link = document.createElement("a")
                link.download = filename
                link.style.displya = "none"
                link.href = URL.createObjectURL(blob)
                document.body.appendChild(link)
                link.click()
                URL.revokeObjectURL(link.href)
                document.body.removeChild(link)
            }else{
                navigator.msSaveBold(blob,filename)
            }
        },
        MultipleLearningGetyear:function(val){
            this.MultipleLearning.year_id = val
        },
        MultipleLearningChangeGrdId(val){
            if(val==3){
                this.MultipleLearning.type = "35"
            }
            else{
                this.MultipleLearning.type = "25"
            }
        },
        MultipleLearningExportExcel(){
            const _self = this
            const apiurl = `${_self.$apiroot}${_self.urlObject.MultipleLearning}`

            if(_self.MultipleLearning.year_id==null || _self.MultipleLearning.grd_id==null){
                _self.$message.error("請選擇學年及年級！")
                return false;
            }

            const loading = _self.$loading({
                lock: true,
                text: '資料讀取中，請稍後。',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });

            _self.axios.get(
                apiurl,
                {
                    params: _self.MultipleLearning,
                    responseType: 'blob',
                    headers:{'SkyGet':_self.$token}
                }
            )
            .then((res)=>{
                _self.saveAs(res, _self.MultipleLearning.type)
            })
            .catch((error)=>{
                _self.$message.error('發生錯誤:'+error)
            })
            .finally(()=>loading.close())
        },
        LearningResultGetyear:function(val){
            this.LearningResult.year_id = val
        },
        LearningResultChangeGrdId(val){
            if(val==3){
                this.LearningResult.type = "36"
            }
            else{
                this.LearningResult.type = "26"
            }
        },
        LearningResultExportExcel(){
            const _self = this
            const apiurl = `${_self.$apiroot}${_self.urlObject.LearningResult}`

            if(_self.LearningResult.year_id==null || _self.LearningResult.grd_id==null || _self.LearningResult.type==null){
                _self.$message.error("請選擇學年、年級、類別！")
                return false;
            }

            const loading = _self.$loading({
                lock: true,
                text: '資料讀取中，請稍後。',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });

            _self.axios.get(
                apiurl,
                {
                    params:_self.LearningResult,
                    responseType: 'blob',
                    headers:{'SkyGet':_self.$token}
                }
            )
            .then((res)=>{
                _self.saveAs(res, _self.LearningResult.type)
            })
            .catch((error)=>{
                _self.$message.error('發生錯誤:'+error)
            })
            .finally(()=>loading.close())
        },
        StdPositionFromSchGetyear:function(val){
            this.StdPositionFromSch.year_id = val
        },
        StdPositionFromSchGetsms:function(val){
            this.StdPositionFromSch.sms_id = val
        },
        StdPositionFromSchChangeGrdId(val){
            if(val==3){
                this.StdPositionFromSch.type = "34"
            }
            else{
                this.StdPositionFromSch.type = "24"
            }
        },
        StdPositionFromSchExportExcel(){
            const _self = this
            const apiurl = `${_self.$apiroot}${_self.urlObject.StdPositionFromSch}`

            if(_self.StdPositionFromSch.year_id==null || _self.StdPositionFromSch.sms_id==null || _self.StdPositionFromSch.grd_id==null || _self.StdPositionFromSch.type==null){
                _self.$message.error("請選擇學年、學期、年級！")
                return false;
            }

            const loading = _self.$loading({
                lock: true,
                text: '資料讀取中，請稍後。',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });

            _self.axios.get(
                apiurl,
                {
                    params:_self.StdPositionFromSch,
                    responseType: 'blob',
                    headers:{'SkyGet':_self.$token}
                }
            )
            .then((res)=>{
                _self.saveAs(res, _self.StdPositionFromSch.type)
            })
            .catch((error)=>{
                _self.$message.error('發生錯誤:'+error)
            })
            .finally(()=>loading.close())
        }
    },
    async beforeMount(){
        
    },
    mounted() {
    }
  }
</script>
  