<template>
    <div>
        <el-dialog title="志工服務記錄"
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
                        <el-form-item label="服務名稱：" prop="volunteer_name">
                          <el-input v-model="form.volunteer_name" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="服務單位：" prop="volunteer_unit">
                          <el-input v-model="form.volunteer_unit" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
                        </el-form-item>
                    </el-col>                    
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="開始日期：" prop="startdate">
                            <el-date-picker :clearable="true" v-model="form.startdate" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                v-on:change="begdate_chk_range"
                                v-on:focus="begdate_chk_value"
                                :readonly=" is_readonly">
                            </el-date-picker>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="結束日期：" prop="enddate">
                            <el-date-picker :clearable="true" v-model="form.enddate" type="date"
                                format="yyyy 年 MM 月 dd 日" :editable="false" value-format="yyyyMMdd"
                                style="width:75%;margin-right:100%;"
                                v-on:change="enddate_chk_range"
                                v-on:focus="enddate_chk_value"
                                :readonly=" is_readonly">
                            </el-date-picker>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="時數" prop="hours">
                            <el-input v-model="form.hours" placeholder="只能輸入整數位" style="width:75%;margin-right:100%;" :readonly="(form.is_sys === '1' || edit_model === 'edit')?true:false"></el-input>
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
                const validateHours = (rule, value, callback) => {
                    //let req1 = /^[0-9]*$/
                    //let req1 = /^(([1-9][0-9]\d{3})|(([0].\d{0,2}|[1-9][0-9]\d{3}.\d{0,2})))$/
                    let req1 = /^\d{0,3}.\d{0,2}$/
                    if (!req1.test(value)) {
                        callback(new Error('只能輸入數字'))
                        this.$set(this.form, "hours", '');
                    }
                    callback();
                }                    
                return {
                            sms_type: '1',
                            form: {
                                    year_id:'',
                                    sms_id:'',
                                    volunteer_name:'',
                                    volunteer_unit:'',
                                    startdate:'',
                                    enddate:'',
                                    hours:'',
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
                                volunteer_unit: [{ required: true, message: '請輸入服務單位', trigger: 'change' }],
                                volunteer_name: [
                                    { required: true, message: '請輸入服務名稱', trigger: 'change' }
                                ],
                                startdate: [
                                    { required: true, message: '請輸入開始日期', trigger: 'change' }
                                ],
                                enddate: [
                                    { required: true, message: '請輸入結束日期', trigger: 'change' }
                                ],
                                hours: [
                                    { required: true, message: '請輸入時數', trigger: 'change' },
                                    { type: 'string', required: true, trigger: 'blur', validator: validateHours }
                                ],
                                content: [
                                    { required: true, message: '請輸入內容簡述', trigger: 'change' }
                                ]
                            },
                            dialogFormVisible: this.dialog_show,
                            is_readonly :false,
                            method_type:"post",
                            SaveButtonVisible:false   
                         }
            },
            methods: {
                    cancel:function(){
                        this.dialogFormVisible = false
                    },
                    save:function(){
                        let _self = this
                        let formdata = new FormData();

                        this.data_structure.header.forEach(function(value, index, array){
                                formdata.append(value.col,_self.form[value.col]);
                            });
                        formdata.append("token",_self.$token)

                        _self.$refs["ruleForm"].validate((valid) => {
                            if (valid) {
                                const loading = _self.$loading({
                                        lock: true,
                                        text: '資料讀取中，請稍後。',
                                        spinner: 'el-icon-loading',
                                        background: 'rgba(0, 0, 0, 0.7)'
                                        });

                                const apiurl = _self.api_interface.save_form
                                _self.$http({
                                    url:apiurl,
                                    method:_self.method_type,
                                    data:formdata,
                                    headers:{'SkyGet':_self.$token}
                                    })
                                    .then((res)=>{
                                                    if (res.data.status == 'Y'){
                                                        _self.$message.success('存檔成功!!')
                                                    }else{
                                                        _self.$message.error(res.data.message)
                                                    }
                                            })
                                    .catch((error) => {
                                        _self.$message({
                                        message: '存檔失敗:'+error,
                                        type: 'error',
                                        duration:0,
                                        showClose: true,
                                        })
                                    })
                                    .finally(()=>loading.close())
                                this.dialogFormVisible = false
                            }
                            else{
                                return false
                            }
                        })
                    },
                    close:function() {
                        this.$emit('get-show', false)
                    },
                    getyear:function(val){
                        this.form.year_id = val
                    },
                    getsms:function(val){
                         this.form.sms_id = val
                    },
                    enddate_chk_range:function(){
                        if (this.form.startdate > this.form.enddate) {
                            this.form.enddate = ''
                            this.$message.error('結束日期不可小於開始日期，請確認。');
                        }
                        if (this.form.startdate === "" || this.form.startdate === null) {
                            this.form.enddate = ''
                        }
                    },
                    enddate_chk_value:function(){
                        if (this.form.startdate === "" || this.form.startdate === null) {
                            this.$message.error('請先輸入開始日期，請確認。');
                            this.form.enddate = ''
                        }
                    },
                    begdate_chk_range:function(){
                        if (this.form.enddate != "" && this.form.enddate != null) {
                            if (this.form.startdate > this.form.enddate) {
                                 this.form.startdate = ''
                                 this.form.enddate = ''
                                 this.$message.error('開始日期不可大於結束日期，請確認。');
                            }
                        }

                        if(this.form.startdate === null){
                            this.form.startdate = ''
                            this.form.enddate = ''
                            this.$message.error('開始日期不可大於結束日期，請確認。');
                        }
                    },
                    begdate_chk_value:function(){
                    }
            },
            mounted() {
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
                    const loading = _self.$loading({
                                    lock: true,
                                    text: '資料讀取中，請稍後。',
                                    spinner: 'el-icon-loading',
                                    background: 'rgba(0, 0, 0, 0.7)'
                                    });

                    const apiurl = _self.api_interface.get_form
                    _self.$http({
                        url:apiurl,
                        method:'get',
                        params:this.parameter,
                        headers:{'SkyGet':_self.$token}
                        })
                        .then((res)=>{
                                        if (res.data.status == 'Y'){
                                            this.data_structure.header.forEach(function(value, index, array){
                                                if(res.data.dataset[0][value.col]){
                                                    if(value.col == "year_id" || value.col == "sms_id"){
                                                        _self.form[value.col] = res.data.dataset[0][value.col].toString()
                                                    }else if(value.col == "startdate" || value.col == "enddate"){
                                                        _self.form[value.col] = res.data.dataset[0][value.col] == "undefined" ? "":res.data.dataset[0][value.col]
                                                    }else{
                                                        _self.form[value.col] = res.data.dataset[0][value.col]
                                                    }
                                                }
                                            })
                                            _self.is_readonly = (res.data.dataset[0].is_sys === "1"? true : false)
                                        }else{
                                            _self.$message.error('查無資料，請確認')
                                        }
                                })
                        .catch((error) => {
                            _self.$message({
                            message: '系統發生錯誤'+error,
                            type: 'error',
                            duration:0,
                            showClose: true,
                            })
                        })
                        .finally(()=>loading.close())
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
