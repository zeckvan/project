import { apiHelper } from '@/utils/provider_axios.js'

const StuFileInfo = {
    file_data:(parameter)=>{
        return apiHelper.get('StuFileInfo/files',{params:parameter})
    },
    file_download:(parameter)=>{
        return apiHelper.get('StuFileInfo/file',{params:parameter,responseType: 'blob'})
    },
    file_delete:(parameter)=>{
        return apiHelper.delete('StuFileInfo/file',{data:parameter,responseType: 'blob'})
    },
    file_upload:(formdata)=>{
        return apiHelper.post('StuFileInfo/file',formdata)
    }
}

export{StuFileInfo}