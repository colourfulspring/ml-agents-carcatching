import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns
import pandas as pd
# 生成数据
x = np.arange(5)

y_a = np.random.uniform(size=5)
y_b = np.random.uniform(size=5)

label_a = np.full(x.shape, fill_value='a')
label_b = np.full(x.shape, fill_value='b')

data_a = pd.DataFrame(np.concatenate((x[:, None], label_a[:, None], y_a[:, None]), axis=1),
                      columns=['x', 'label', 'y'])
data_b = pd.DataFrame(np.concatenate((x[:, None], label_b[:, None], y_b[:, None]), axis=1),
                      columns=['x', 'label', 'y'])
data = pd.concat([data_a, data_b], axis=0)

# x、y轴数据需要为数字类型，但上面的操作后会变成object，所以要进行一下转换
data[['x', 'y']] = data[['x', 'y']].apply(pd.to_numeric)
print(data)
# 设置样式
sns.set_theme(context='paper', style='darkgrid')
fig = plt.figure()
plt.title('multi lines')
# 绘图
sns.lineplot(x="x", y="y", data=data, errorbar=('sd', 1))
plt.show()
