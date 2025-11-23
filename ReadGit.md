## Git初始化+双分支+关联远程仓库操作详细步骤

### 第一步：在Gitee和GitHub创建远程仓库

#### 1.在Gitee上创建仓库：

1. 登录你的Gitee账号
2. 点击右上角 + 号，选择 "新建仓库" 
3. 填写仓库名称（例如 my-project），选择 "公开" 或 "私有"
4. 关键点：在"初始化仓库"部分，不要勾选 "使用Readme文件初始化这个仓库"、"设置模板"或"设置分支" 。我们希望推送一个已存在的本地仓库，因此远程仓库初始应为空
5. 点击"创建"

#### 2.在GitHub上创建仓库：

1. 登录你的GitHub账号
2. 点击右上角 + 号，选择 "New repository" 
3. 填写仓库名称（建议与Gitee仓库名一致，例如 my-project）
4. 同样地，不要勾选 "Add a README file"、"Add .gitignore" 或 "Choose a license"
5. 点击"Create repository"

### 第二步：初始化本地仓库并进行首次提交

接下来，我们在本地初始化Git仓库，并完成第一次提交

1. 进入指定项目目录后，打开Git终端进行**Git初始化**，命令: ``git init``

2. (可选)创建并配置``.gitignore``文件。这个文件用于告诉Git哪些文件或目录不需要纳入版本管理，例如依赖包、编译产物、本地配置文件等。

3. 添加用户邮箱和昵称
   
   ```
   git config --global user.email "you@example.com"
   git config --global user.name "Your Name"
   ```

4. **将项目文件添加到暂存区**。命令: ``git add .`` 或 ``git add -A``

5. **提交文件到本地仓库**。命令: ``git commit -m "初始提交：项目基础结构"``，-m 后面是本次提交的说明信息，建议清晰描述本次提交的内容，是必选项

### 第三步：关联本地仓库与两个远程仓库

现在，我们将本地仓库与刚才在Gitee和GitHub上创建的两个远程仓库关联起来。这里我们为它们分别起一个易于区分的"别名"（remote name）

1. **关联Gitee远程仓库**，为其设置别名为 gitee。命令: ``git remote add gitee git@gitee.com:你的用户名/你的仓库名.git``
2. **关联GitHub远程仓库**，我们为其设置别名为 `github`。命令: `git remote add github git@github.com:你的用户名/你的仓库名.git`
3. **验证远程仓库是否添加成功** ，命令: `git remote -v`

### 第四步：创建并管理主分支与开发分支

我们将创建并初始化主分支（如 `main` 或 `master`），以及一个开发分支（如 `develop`）

1. **创建并切换到开发分支 `develop`**，使用 `-b` 选项可以创建并立即切换到新分支。命令: `git checkout -b develop`，现在你就在 `develop` 分支上进行开发了。可以在此分支上继续进行你的代码编写和修改

2. **推送开发分支到两个远程仓库**，当在 `develop` 分支上完成了一些开发工作并提交后，可以将其推送到远程。首次推送时需要使用 `-u` (或 `--set-upstream`) 参数建立本地分支与远程分支的跟踪关系。命令: 
   
   ```
   git push -u gitee develop
   git push -u github develop
   ```
   
   这会在Gitee和GitHub上创建名为 `develop` 的远程分支

3. **切换回主分支**，假设你的主分支名为 `main` (或 `master`)，切换回去。命令：`git checkout main   # 或 git checkout master`

4. **将开发分支合并到主分支**，当 `develop` 分支的功能开发完成并测试稳定后，可以将其合并到主分支。命令: `git merge develop`

### 第五步：推送代码到多个远程仓库

现在，关键的步骤来了：如何将本地分支的更新（无论是主分支还是开发分支）一次性推送到Gitee和GitHub

1. **方法一：分别推送（清晰明了）**
   
   这是最直接的方法，分别指定远程仓库别名和分支名进行推送 
   
   ```
   # 推送主分支到两个远程
   git push gitee main # 或 master
   git push github main # 或 master
   
   # 推送开发分支到两个远程
   git push gitee develop
   git push github develop
   
   # 以下代码可选（我没做）
   # 远程仓库更新默认分支后
   git fetch origin
   git remote set-head origin -a
   
   # 验证分支跟踪关系
   git branch -vv
   ```

2. **方法二：配置单个Origin同时推送到两个地址**
   
   你也可以只为默认的 `origin` 配置多个推送URL，这样 `git push origin 分支名` 就会同时推送到两个仓库
   
   ※ 首先，删除刚才为 `gitee` 和 `github` 设置的远程（如果你打算用此方法）
   
   ```
   git remote remove gitee
   git remote remove github
   ```
   
   ※ 添加 `origin` 并设置多个推送URL
   
   ```
   `git remote add origin git@gitee.com:你的用户名/你的仓库名.git`
   `git remote set-url --add --push origin git@gitee.com:你的用户名/你的仓库名.git
   `git remote set-url --add --push origin git@github.com:你的用户名/你的仓库名.git
   ```
   
   ※ 之后推送时，只需
   
   ```
   git push origin main # 同时推送到Gitee和GitHub的main分支
   git push origin develop # 同时推送到Gitee和GitHub的develop分支
   ```

## develop分支开发完成后合并到master分支详细步骤

### 第一步：合并前准备工作

1. **确保 develop 分支已提交所有更改**
   
   ※ 切换到 develop 分支
   
   `git checkout develop`
   
   ※ 检查状态，确保没有未提交的更改
   
   `git status`
   
   ※ 如果有未提交的更改，先提交
   
   `git add .`
   `git commit -m "完成功能开发"`

2. **（可选但推荐）推送最新的 develop 分支到远程**
   
   ※ 确保远程仓库有最新的 develop 分支代码
   
   `git push origin develop`
   
   ※ 或者分别推送到 gitee 和 github
   
   `git push gitee develop`
   `git push github develop`

### 第二步：合并操作

1. **(单人开发用不到)更新本地 develop 分支到最新状态**
   
   ※ 切换到 develop 分支
   
   `git checkout develop`
   
   ※ 拉取远程最新代码（如果其他人也在开发）
   
   `git pull gitee develop` 
   
   或
   
   `git pull github develop`

2. **切换到主分支并更新**
   
   ※ 切换到主分支（假设主分支叫 master）
   
   `git checkout master`
   
   ※ 拉取远程主分支的最新代码
   
   ```
   git pull gitee master
   git pull github master
   ```

3. **执行合并操作**
   
   ※ 将 develop 分支合并到当前分支（master）
   
   `git merge develop`

### 第三步：处理合并冲突

1. **识别冲突文件**
   
   ※ 查看哪些文件有冲突
   
   `git status`
   
   输出会显示类型：
   
   ```
   Unmerged paths:
     both modified:   src/main.py
     both modified:   README.md
   ```

2. **手动编辑冲突文件**
   
   ※ 用编辑器打开有冲突的文件，你会看到类似这样的标记：
   
   ```
   <<<<<<< HEAD
   # 主分支的代码
   print("这是主分支的内容")
   =======
   # develop 分支的代码  
   print("这是开发分支的新功能")
   >>>>>>> develop
   ```

3. **决定保留哪部分代码**
   
   - 删除 `<<<<<<< HEAD`、`=======` 和 `>>>>>>> develop` 标记
   
   - 保留你想要的代码，或者结合两者的代码
   
   修改后：
   
   ```
   # 合并后的代码
   print("这是结合了两个分支的最终内容")
   ```

4. **标记冲突已解决**
   
   ```
   # 添加已解决的文件
   git add src/main.py
   git add README.md
   
   # 或者添加所有已解决的文件
   git add .
   ```

5. **完成合并提交**
   
   `git commit -m "合并develop分支到main，解决代码冲突"`

6. **推送合并后的主分支**
   
   ※ 合并完成后，将更新后的主分支推送到两个远程仓库：
   
   ```
   # 推送到 Gitee
   git push gitee main
   
   # 推送到 GitHub
   git push github main
   
   # 如果你想一次推送所有分支到某个远程，可以使用 --all 参数
   git push gitee --all
   git push github --all
   ```

7. **验证合并结果**
   
   ※ 检查本地状态
   
   ```
   # 查看提交历史
   git log --oneline --graph -10
   
   # 输出示例：
   # *   a1b2c3d (HEAD -> main, gitee/main, github/main) 合并develop分支到main
   # |\  
   # | * d4e5f6g (develop) 完成新功能开发
   # | * e7f8h9i 修复某个bug
   # * | j0k1l2m 主分支的某个更新
   # |/  
   # * n3o4p5q 初始提交
   ```

8. **检查远程仓库**
   
   - 访问 Gitee 和 GitHub 网站
   
   - 查看主分支的提交历史
   
   - 确认最新的合并提交已显示

9. **（可选）后续操作**

10. **更新 develop 分支**
    
    ※ 合并后，你可能想将主分支的更新同步回 develop 分支：
    
    ```
    # 切换到 develop 分支
    git checkout develop
    
    # 合并主分支的最新更改到 develop
    git merge main
    
    # 推送更新后的 develop 分支
    git push gitee develop
    git push github develop
    ```

11. **删除已合并的功能分支（如果适用）**
    
    ※ 如果你有临时功能分支已经合并到 develop，现在可以删除：
    
    ```
    # 删除本地功能分支
    git branch -d feature/xxx
    
    # 删除远程功能分支
    git push gitee --delete feature/xxx
    git push github --delete feature/xxx
    ```

12. **完整命令序列**
    
    ```
    # 1. 准备阶段
    git checkout develop
    git status
    git add .
    git commit -m "完成最终开发"
    git push gitee develop
    git push github develop
    
    # 2. 合并阶段
    git checkout main
    git pull gitee main
    git pull github main
    git merge develop
    
    # 3. 如果有冲突，解决冲突
    # git status              # 查看冲突文件
    # 手动编辑冲突文件...
    # git add .              # 标记冲突解决
    # git commit -m "解决合并冲突"
    
    # 4. 推送阶段
    git push gitee main
    git push github main
    
    # 5. 验证
    git log --oneline --graph -5
    ```

13. .......