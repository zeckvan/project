<template>
    <div>
        <el-dialog title="競賽參與紀錄"
            :visible.sync="dialogFormVisible"
            v-on:close="close"
            :fullscreen="false" width="100%"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <el-form ref="ruleForm" :model="form" :rules="rules" label-width="120px">
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="學年度：" prop="year_id">
                            <datalistyear v-on:get-year="getyear"  :year_id="form.year_id" placeholder="" style="width:50%;margin-right:100%;" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="學期：" prop="sms_id" placeholder="" style="width:50%;margin-right:100%;">
                            <datalistsms v-on:get-sms="getsms"   :sms_type="sms_type" :sms_id="form.sms_id" :disabled="(form.is_sys === '1' || edit_model === 'edit')? true:false"/>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="競賽名稱" prop="competition_name">
                          <el-input v-model="form.competition_name" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>                        
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="競賽項目" prop="competition_item">
                            <el-input v-model="form.competition_item" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="競賽領域" prop="competition_area">
                          <el-select v-model="form.competition_area" placeholder="" style="width:75%;margin-right:100%;" :disabled="is_readonly? true:false">
                                <el-option v-for="item in area_id" :key="item.id" :label="item.name" :value="item.id" >
                                </el-option>
                            </el-select>
                        </el-form-item>                        
                    </el-col>     
                    <el-col :span="8">
                        <el-form-item label="競賽等級" prop="competition_grade">
                          <el-select v-model="form.competition_grade" placeholder="" style="width:75%;margin-right:100%;" :disabled="is_readonly? true:false">
                                <el-option v-for="item in grade_id" :key="item.id" :label="item.name" :value="item.id" >
                                </el-option>
                            </el-select>
                        </el-form-item>                                                                                 
                    </el-col>                                        
                    <el-col :span="8">
                        <el-form-item label="獎項" prop="competition_result">
                          <el-select v-model="form.competition_result" placeholder="" style="width:75%;margin-right:100%;" :disabled="is_readonly? true:false">
                                <el-option v-for="item in result_id" :key="item.id" :label="item.name" :value="item.id" >
                                </el-option>
                            </el-select>
                        </el-form-item>                             
                    </el-col>                                        
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="參與類型" prop="competition_type">
                            <el-select v-model="form.competition_type" placeholder="" style="width:75%;margin-right:100%;" :disabled="is_readonly? true:false">
                                <el-option v-for="item in type_id" :key="item.id" :label="item.name" :value="item.id" >
                                </el-option>
                            </el-select>                            
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="結果公布日期" prop="competition_date">
                            <el-date-picker :clearable="true" v-model="form.competition_date" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                :readonly=" is_readonly">
                            </el-date-picker>
                        </el-form-item>
                    </el-col>                    
                </el-row>                
                <el-row>                    
                  <el-form-item label="內容簡述：" prop="content">
                    <el-input v-model="form.content" type="textarea" rows = 5></el-input>
                  </el-form-item>
                </el-row>
            </el-form>
            <div v-if="SaveButtonVisible==false">
                <el-button type="primary" @click="save" :disabled="SaveButtonVisible">確定存檔</el-button>
                <el-button type="primary" @click="cancel">關閉視窗</el-button>
            </div>
            <div v-else>
                <el-button type="primary" @click="cancel">關閉視窗</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script type="module">
    import datalistyear from '@/components/pub/DataList_year.vue'
    import datalistsms from '@/components/pub/DataList_sms.vue'
    import * as stuAPI from  '@/apis/stuApi.js' 

    export default {
            props: {
                dialog_show: {
                    type: Boolean
                },
                edit_model:{
                    tyep:String
                },
                data_structure:{
                    type:Object
                },
                parameter:{
                    type:Object
                },
                userStatic: {
                    type: Object,
                },
                api_interface:{
                    type: Object,
                },
                formSaveCheck:{
                    type:String
                }
            },
            data() {
                return {
                            sms_type: '1',
                            form: {
                                    year_id:'',
                                    sms_id:'',
                                    std_no:'',
                                    ser_id:'',
                                    competition_name:'',
                                    competition_item:'',
                                    competition_area:'',
                                    competition_grade:'',
                                    competition_result:'',
                                    competition_date:'',
                                    competition_type:'',
                                    content:'',
                                    is_sys:''
                                   },
                            rules: {
                                year_id: [
                                        { required: true, message: '請輸入學年', trigger: 'change' }
                                    ],
                                sms_id: [
                                    { required: true, message: '請輸入學期', trigger: 'change' }
                                ],
                                competition_name: [
                                    { required: true, message: '請輸競賽名稱', trigger: 'change' }
                                ],
                                competition_area: [
                                    { required: true, message: '請輸入競賽領域', trigger: 'change' }
                                ],
                                competition_grade: [
                                    { required: true, message: '請輸入競賽等級', trigger: 'change' }
                                ],
                                competition_result: [
                                    { required: true, message: '請輸入獎項', trigger: 'change' }
                                ],
                                competition_type: [
                                    { required: true, message: '請輸入參與類型', trigger: 'change' }
                                ],
                                content: [
                                    { required: true, message: '請輸入內容簡述', trigger: 'change' }
                                ],
                                competition_date: [
                                    { required: true, message: '請輸入結果公布日期', trigger: 'change' }
                                ]
                            },
                            dialogFormVisible: this.dialog_show,
                            is_readonly :false,
                            method_type:"post",
                            area_id:[{id:"跨領域",name:"跨領域"},
                                     {id:"不分領域",name:"不分領域"},
                                     {id:"其它領域",name:"其它領域"},
                                    ],
                            grade_id:[  {id:"1",name:"校級"},
                                        {id:"2",name:"縣市級"},
                                        {id:"3",name:"全國"},
                                        {id:"4",name:"國際"}
                                     ],                                    
                            result_id:[ {id:"未得獎",name:"未得獎"},
                                        {id:"第一名",name:"第一名"},
                                        {id:"第二名",name:"第二名"},
                                        {id:"第三名",name:"第三名"},
                                        {id:"金牌獎",name:"金牌獎"},
                                        {id:"佳作",name:"佳作"}, 
                                        {id:"優勝",name:"優勝"},                                         
                                     ],                                                         
                            type_id:[  {id:"1",name:"個人參與"},
                                       {id:"2",name:"團體參與"}
                                    ],
                            SaveButtonVisible:false 
                         }
            },
            methods: {
                    cancel:function(){
                        this.dialogFormVisible = false
                    },
                    save:async function(){
                        let _self = this
                        let formdata = new FormData();
                        let valid_pass = 'N'

                        this.data_structure.header.forEach(function(value, index, array){
                                formdata.append(value.col,_self.form[value.col]);
                            });
                        formdata.append("token",_self.$token)

                        _self.$refs["ruleForm"].validate((valid) => {
                            if (valid) {
                                valid_pass = 'Y'
                            }
                            else{
                                return false
                            }
                        })

                        if(valid_pass == 'Y')
                        {
                            const { data, statusText } = await stuAPI.StuCompetition.Put(formdata) 

                            if (statusText !== "OK") {
                                throw new Error(statusText);
                            }        

                            if (data.status == 'Y'){                                  
                                _self.$message.success('存檔成功!!')
                            }else{
                                _self.$message.error(data.message)
                            }     
                            this.dialogFormVisible = false
                        }
                    },
                    close:function() {
                        this.$emit('get-show', false)
                    },
                    getyear:function(val){
                        this.form.year_id = val
                    },
                    getsms:function(val){
                         this.form.sms_id = val
                    }
            },
            async mounted() {
                let _self = this
                if(this.formSaveCheck == 'studiversecheck'){
                    this.SaveButtonVisible = true
                }else{
                    this.SaveButtonVisible = false
                }
                if(this.edit_model === "add"){
                    this.is_readonly = false
                    this.method_type = "post"
                    this.data_structure.header.forEach(function(value, index, array){
                                                _self.form[value.col] = ''
                                          });
                    this.form.is_sys = "2"
                }else{
                        const { data, statusText } = await stuAPI.StuCompetition.Get(_self.parameter) 

                        if (statusText !== "OK") {
                            throw new Error(statusText);
                        }

                        if (data.status == 'Y'){
                            this.data_structure.header.forEach(function(value, index, array){
                                if(data.dataset[0][value.col]){
                                    if(value.col == "year_id" || value.col == "sms_id"){
                                        _self.form[value.col] = data.dataset[0][value.col].toString()
                                    }else if(value.col == "startdate" || value.col == "enddate"){
                                        _self.form[value.col] = data.dataset[0][value.col] == "undefined" ? "":data.dataset[0][value.col]
                                    }else{
                                        _self.form[value.col] = data.dataset[0][value.col]
                                    }
                                }
                            })
                            _self.is_readonly = (data.dataset[0].is_sys === "1"? true : false)
                        }else{
                            _self.$message.error('查無資料，請確認')
                        }

                        this.method_type = "put"
                    }
            },
            components: {
                datalistyear,
                datalistsms
            },
    }
</script>

<style>

</style>
