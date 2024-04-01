import { apiHelper } from '@/utils/provider_axios.js'

const TeaAttestation = {
    MultipleLearning:{
        Get:(parameter)=>{
            return apiHelper.get('TeaAttestation/MultipleLearning',{params:parameter})
        }
    }
}

export{TeaAttestation}
