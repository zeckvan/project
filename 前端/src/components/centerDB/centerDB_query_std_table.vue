<template>
  <div>
      <el-table style="width: 100%"
        :data="$props.data"
        row-key="id"
        border
      >
        <el-table-column v-for="item in render_header"
          :prop="item.prop"
          :label="item.label"
          :width="item.width"
        >
        </el-table-column>
      </el-table>
  </div>
</template>

<script type="module">
import data_structure from "@/js/centerDB_std_detial_structure.js"

export default {
  props: {
    data: {
      type: Array
    },
    name: {
      type: String
    }
  },
  data() {
    return {
    }
  },
  methods: {
  },
  components: {
  },
  mounted() {
  },
  beforeMount() {
  },
  computed: {
    render_header() {
      let headers = [{prop:"", label:"", width:""}]
      if(data_structure[this.$props.name] != null || data_structure[this.$props.name] != undefined)
      {
        headers = data_structure[this.$props.name].filter((a) => { return a.hidden === 'N' }).sort((a, b) => {
          if (a.sort > b.sort) {
            return 1
          } else {
            return -1
          }
        })
      }
      
      return headers
    }
  }
}
</script>

<style></style>
