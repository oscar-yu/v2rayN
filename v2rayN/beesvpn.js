/*
 * @Author       : oscar.yu
 * @Date         : 2024-10-14 10:46:19
 * @LastEditors  : oscar.yu
 * @LastEditTime : 2024-10-14 11:05:25
 * @Description  : BeesVPN自动获取订阅地址
 */
// 定义用于生成设备ID的函数
function generateDeviceId() {
  var prefix = '001168.'
  var id = prefix + Array.from({ length: 32 }, () => Math.floor(Math.random() * 16).toString(16)).join('')
  return id
}

// 定义用于登录并获取Token的函数
function loginAndGetToken(loginUrl, deviceId, callback) {
  const headers = {
    'Content-Type': 'application/json; charset=utf-8',
    'User-Agent': 'BeesVPN/2 CFNetwork/1568.100.1 Darwin/24.0.0',
    Accept: '*/*',
    'Accept-Language': 'zh-CN,zh-Hans;q=0.9'
  }
  const payload = {
    invite_token: '',
    device_id: deviceId
  }
  const params = {
    url: loginUrl,
    headers: headers,
    body: JSON.stringify(payload)
  }
  $httpClient.post(params, (errormsg, response, data) => {
    if (response.status == 200) {
      console.log('GetToken成功')
      let result = JSON.parse(data)
      const token = result.data ? result.data.token : null
      callback(token)
    } else {
      console.log('GetToken失败：' + errormsg)
    }
  })
}

// 定义用于获取并处理订阅的函数
function fetchAndProcessSubscription(subscribeUrl, token, callback) {
  const headers = {
    Accept: '*/*',
    'User-Agent': 'BeesVPN/2 CFNetwork/1568.100.1 Darwin/24.0.0'
  }

  let params = {
    url: subscribeUrl + '?token=' + token,
    headers: headers
  }
  $httpClient.get(params, (errormsg, response, data) => {
    if (response.status == 200) {
      console.log('FetchSubscription成功')
      let result = JSON.parse(data)
      let urls = []
      if (result.data) {
        result.data.forEach(item => {
          item.list.forEach(subItem => {
            const url = subItem.url ? subItem.url.replace('vless:/\\/', 'vless://') : ''
            if (url) {
              urls.push(url)
            }
          })
        })
      }
      callback(urls)
    } else {
      console.log('FetchSubscription失败：' + errormsg)
    }
  })
}

// 转换为订阅地址
function postToDpaste(encodedContent) {
  let params = {
    url: 'https://dpaste.com/api/',
    body: JSON.stringify({ expiry_days: 3, content: encodedContent })
  }
  $httpClient.post(params, (errormsg, response, data) => {
    if (response.status == 200) {
      let dpasteUrl = data.trim() + '.txt'
      console.log('恭喜你成功获得订阅：' + dpasteUrl)
    } else {
      console.log('获取订阅地址失败：' + errormsg)
    }
  })
}
function encodeBase64(str) {
    return btoa(str);
}
function decodeBase64(encodedStr) {
    return atob(encodedStr);
}
// 主函数
function main() {
  const apiUrl = 'https://94.74.97.241/api/v1'
  const loginEndpoint = '/passport/auth/loginByDeviceId'
  const subscribeEndpoint = '/client/appSubscribe'
  const deviceId = generateDeviceId()

  loginAndGetToken(apiUrl + loginEndpoint, deviceId, function (token) {
    if (!token) {
      $notification.post('登录失败', '未能获取到token', '')
      return
    }
    fetchAndProcessSubscription(apiUrl + subscribeEndpoint, token, function (urls) {
      if (!urls || urls.length === 0) {
        $notification.post('获取订阅失败', '未能获取到订阅链接', '')
        return
      }
      const content = urls.join('\n')
      const encodedContent = encodeBase64(content);
      // $notification.post('订阅成功', '获取到订阅链接', encodedContent)
      postToDpaste(encodedContent)
    })
  })
}

main()
