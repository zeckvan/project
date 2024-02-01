<template>
  <div>
    <el-descriptions class="margin-top" :column="1" border>
      <el-descriptions-item>
        <template slot="label">
          名冊
        </template>
        {{ file_info }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">
          上傳人員
        </template>
        {{ $props.stdObject["上傳人員"] }}
      </el-descriptions-item>
      <el-descriptions-item>
        <template slot="label">
          開放確認期限
        </template>
        {{ $props.stdObject["開放確認期限"] }}
        <el-button
          v-if="$props.std_urlObject!=undefined && $props.stdObject.kind != null"
          type="success"
          @click="stdCheckData($props.stdObject.kind)"
        >資料正確無誤</el-button>
      </el-descriptions-item>
    </el-descriptions>

    <el-tabs v-model="activeName">
      <el-tab-pane v-for="(cls, index) in tableData" :label="cls.name" :key="index" :name="index+''">
        <myTable :data="cls.detail" :name="cls.name"></myTable>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script type="module">
import myTable from "@/components/centerDB/centerDB_query_std_table.vue"

export default {
  props: {
    stdObject: {
      type: Object,
    },
    std_urlObject: {
      type: Object,
    }
  },
  data() {
    return {
      //共用
      activeName: '0'
    }
  },
  methods: {
    //學生
    stdCheckData(kind){
      this.$emit('std-CheckData',kind)
    },
  },
  components: {
    //共用
    myTable
  },
  mounted() {
  },
  beforeMount() {
  },
  computed: {
    //共用
    file_info() {
      let info = JSON.parse(this.$props.stdObject["名冊資訊"])

      return `${info[0]["名稱"]}(${info[0]["SHA"]})`
    },
    tableData(){
      let name = this.$props.stdObject["名稱"]

      let dataTable = []
      let dateObj = {}
      if(name.indexOf("多元表現") > -1)
      {
        dateObj = this.$props.stdObject["多元表現"]
      }
      else if(name.indexOf("修課紀錄") > -1)
      {
        dateObj = this.$props.stdObject["修課紀錄"]
      }
      else if(name.indexOf("校內幹部") > -1)
      {
        dateObj = this.$props.stdObject["校內幹部經歷"]
      }
      else if(name.indexOf("課程學習") > -1)
      {
        dateObj = this.$props.stdObject["課程學習成果"]
      }
      let keys = Object.keys(dateObj)

      keys.forEach((element, index1) => {
        if(dateObj[element] != null)
        {
          let detail = []
          dateObj[element].forEach((strObj, index2) => {
            let obj = JSON.parse(strObj)
            obj['id'] = `${index1}_${index2}`
            detail.push(obj)
          })

          let data = {
            id: index1,
            name: element,
            detail
          }

          dataTable.push(data)
        }
      })
      // console.log(dataTable)
      return dataTable
    },
  }
}
</script>

<style></style>
