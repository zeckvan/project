<template>
    <div>
        <el-dialog title="彈性學習記錄"
            :visible.sync="dialogFormVisible"
            v-on:close="close"
            :fullscreen="false" width="100%"
            :close-on-press-escape="false" :close-on-click-modal="false">
            <el-form ref="form" :model="form" :rules="rules" label-width="120px">
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
                        <el-form-item label="類別：" prop="type_id">
                          <el-select v-model="form.type_id" placeholder="" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false">
                                <el-option v-for="item in typeid" :key="item.id" :label="item.name" :value="item.id" :disabled="form.is_sys === '1'? true:false">
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="開設名稱：" prop="open_name">
                            <el-input v-model="form.open_name" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="開設單位：" prop="open_unit">
                          <el-input v-model="form.open_unit" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false"></el-input>
                        </el-form-item>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col :span="8">
                        <el-form-item label="每週節數：" prop="hours">
                            <el-input v-model.number="form.hours" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false"></el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :span="8">
                        <el-form-item label="開設週數：" prop="weeks">
                          <el-input v-model.number="form.weeks" @input="checkWeeks" style="width:75%;margin-right:100%;" :disabled="form.is_sys === '1'? true:false"></el-input>
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
        edit_model: {
            tyep: String
        },
        data_structure: {
            type: Object
        },
        parameter: {
            type: Object
        },
        userStatic: {
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
                ser_id:0,
                is_sys:'2',
                year_id:'',
                sms_id:'',
                type_id:'',
                open_name:'',
                open_unit:'',
                hours:'',
                weeks:'',
                content:''
            },
            rules: {
                year_id: [
                    {required: true, message: '請輸入學年', trigger: 'change' }
                ],
                sms_id: [
                    {required: true, message: '請輸入學期', trigger: 'change' }
                ],
                type_id: [
                    { required: true, message: '請輸入類別', trigger: 'change' }
                ],
                open_name: [
                    { required: true, message: '請輸入開設名稱', trigger: 'change' }
                ],
                open_unit: [
                    { required: true, message: '請輸入開設單位', trigger: 'change' }
                ],
                hours: [
                    {required: true, message: '請輸入每週節數(數字)', type:'number', trigger: 'change' }
                ],
                weeks: [
                    {required: true, message: '請輸入開設週數(1-18)', type:'number', trigger: 'change'}
                ],
                content: [
                    {message: '最多100個字', trigger: 'change', max:100}
                ]
            },
            dialogFormVisible: this.dialog_show,
            is_readonly: false,
            method_type: "post",
            typeid: [
                { id: "1", name: "自主學習" },
                { id: "2", name: "選手培訓" },
                { id: "3", name: "充實(增廣)課程" },                
                { id: "4", name: "補強性課程" },
                { id: "5", name: "學校特色活動" },                
            ],
             SaveButtonVisible:false   
        }
    },
    methods: {
        cancel: function () {
            this.dialogFormVisible = false
        },
        save: function () {
            let _self = this
            let formdata = new FormData();

            this.data_structure.header.forEach(function (value, index, array) {
                formdata.append(value.col, _self.form[value.col]);
            });
            formdata.append("token",_self.$token)

            _self.$refs["form"].validate((valid) => {
                if (valid) {
                    const loading = _self.$loading({
                        lock: true,
                        text: '資料讀取中，請稍後。',
                        spinner: 'el-icon-loading',
                        background: 'rgba(0, 0, 0, 0.7)'
                    });

                    const apiurl = `${this.$apiroot}/${this.userStatic.interface}`
                    _self.$http({
                        //'Content-Type': 'multipart/form-data',
                        url: apiurl,
                        method: _self.method_type,
                        data: formdata,
                        headers:{'SkyGet':_self.$token}
                    })
                    .then((res) => {
                        if (res.data.status == 'Y') {
                            _self.$message.success('存檔成功!!')
                        } else {
                            _self.$message.error(res.data.message)
                        }
                    })
                    .catch((error) => {
                        _self.$message.error('存檔失敗:' + error)
                    })
                    .finally(() => loading.close())
                    this.dialogFormVisible = false
                }
                else {
                    return false
                }
            })
        },
        close: function () {
            this.$emit('get-show', false)
        },
        getyear: function (val) {
            this.form.year_id = val
        },
        getsms: function (val) {
            this.form.sms_id = val
        },
        checkWeeks(){
            if(this.form.weeks > 18 || this.form.weeks < 1){ this.form.weeks = " "};
        }
    },
    mounted() {
        let _self = this
        if(this.formSaveCheck == 'studiversecheck'){
                    this.SaveButtonVisible = true
                }else{
                    this.SaveButtonVisible = false
                }
        if (this.edit_model === "add") {
            this.is_readonly = false
            this.method_type = "post"
            this.data_structure.header.forEach(function (value, index, array) {
                _self.form[value.col]
            });
        } else {
            const loading = _self.$loading({
                lock: true,
                text: '資料讀取中，請稍後。',
                spinner: 'el-icon-loading',
                background: 'rgba(0, 0, 0, 0.7)'
            });

            const apiurl = `${this.$apiroot}/${this.userStatic.interface}`
            _self.$http({
                url: apiurl,
                method: 'get',
                params: this.parameter,
                headers:{'SkyGet':_self.$token}
            })
                .then((res) => {
                    if (res.data.status == 'Y') {
                        this.data_structure.header.forEach(function (value, index, array) {
                            if (res.data.dataset[value.col]) {
                                if(value.col == "year_id" || value.col == "sms_id")
                                {
                                    _self.form[value.col] = res.data.dataset[value.col]+""
                                }
                                else
                                {
                                    _self.form[value.col] = res.data.dataset[value.col]
                                }
                            }
                        });
                    } else {
                        _self.$message.error('查無資料，請確認')
                    }
                })
                .catch((error) => {
                    _self.$message.error('系統發生錯誤:' + error)
                })
                .finally(() => loading.close())
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
